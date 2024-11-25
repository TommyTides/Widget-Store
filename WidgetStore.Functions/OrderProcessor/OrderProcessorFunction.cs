using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WidgetStore.Core.Entities;
using WidgetStore.Functions.Models;

namespace WidgetStore.Functions.OrderProcessor;

/// <summary>
/// Azure Function for processing orders
/// </summary>
public class OrderProcessorFunction
{
    private readonly Container _container;
    private readonly ILogger<OrderProcessorFunction> _logger;

    /// <summary>
    /// Initializes a new instance of the OrderProcessorFunction class
    /// </summary>
    /// <param name="container">Cosmos DB container</param>
    /// <param name="logger">Logger instance</param>
    public OrderProcessorFunction(
        Container container,
        ILogger<OrderProcessorFunction> logger)
    {
        _container = container;
        _logger = logger;
    }

    /// <summary>
    /// Processes an order from the queue
    /// </summary>
    /// <param name="messageText">The order processing message</param>
    [Function(nameof(OrderProcessorFunction))]
    public async Task Run(
        [QueueTrigger("orders")] string messageText,
        FunctionContext context)
    {
        try
        {
            _logger.LogInformation("Starting to process message: {MessageText}", messageText);

            // Deserialize the message
            var message = JsonSerializer.Deserialize<OrderProcessingMessage>(messageText);
            _logger.LogInformation("Deserialized message for order: {OrderId}", message?.OrderId);

            if (message == null)
            {
                _logger.LogError("Failed to deserialize message");
                throw new InvalidOperationException("Invalid message format");
            }

            // Get the order using direct item read
            _logger.LogInformation("Fetching order from Cosmos DB: {OrderId}", message.OrderId);
            try
            {
                var orderResponse = await _container.ReadItemAsync<Order>(
                    id: message.OrderId,
                    partitionKey: new PartitionKey("Order"));

                var order = orderResponse.Resource;
                _logger.LogInformation("Found order in Cosmos DB: {OrderId}, Current Status: {Status}, Type: {Type}",
                    order.id,
                    order.Status,
                    order.type);

                // Start processing
                order.Status = "Processing";
                _logger.LogInformation("Updating order status to Processing");
                await UpdateOrderAsync(order);

                // Process each item
                var processingStartTime = DateTime.UtcNow;
                var success = true;

                foreach (var item in message.Items)
                {
                    try
                    {
                        _logger.LogInformation("Processing order item: ProductId: {ProductId}, Quantity: {Quantity}",
                            item.ProductId, item.Quantity);

                        // Get the product - note the Product partition key
                        var response = await _container.ReadItemAsync<Product>(
                            item.ProductId,
                            new PartitionKey("Product"));

                        var product = response.Resource;
                        _logger.LogInformation("Found product: {ProductId}, Current Stock: {Stock}, Type: {Type}",
                            product.id,
                            product.stockQuantity,
                            product.type);  // Changed from Type to type

                        // Validate stock
                        if (product.stockQuantity < item.Quantity)
                        {
                            _logger.LogError("Insufficient stock for product {ProductId}. Required: {Required}, Available: {Available}",
                                product.id, item.Quantity, product.stockQuantity);
                            throw new InvalidOperationException(
                                $"Insufficient stock for product {product.id}. Required: {item.Quantity}, Available: {product.stockQuantity}");
                        }

                        // Update stock
                        product.stockQuantity -= item.Quantity;
                        product.modifiedAt = DateTime.UtcNow;

                        _logger.LogInformation(
                            "Attempting to update product. Id: {Id}, Type: {Type}, Stock: {Stock}",
                            product.id,
                            product.type,
                            product.stockQuantity);

                        await _container.ReplaceItemAsync(
                            product,
                            product.id,
                            new PartitionKey(product.type));  // Changed from Type to type

                        _logger.LogInformation("Updated product stock. New stock level: {Stock}", product.stockQuantity);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing item {ProductId}", item.ProductId);
                        success = false;
                        break;
                    }
                }

                // Update order status and metrics
                order.Status = success ? "Processed" : "Failed";
                if (order.Metrics == null)
                {
                    order.Metrics = new OrderMetrics();
                }

                order.Metrics.ProcessingTimeMinutes =
                    (int)(DateTime.UtcNow - processingStartTime).TotalMinutes;

                _logger.LogInformation("Updating final order status to: {Status}", order.Status);
                await UpdateOrderAsync(order);

                _logger.LogInformation(
                    "Order {OrderId} processing completed with status: {Status}",
                    order.id,
                    order.Status);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogError("Order not found in Cosmos DB: {OrderId}", message.OrderId);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order message: {Error}", ex.Message);
            throw;
        }
    }

    private async Task UpdateOrderAsync(Order order)
    {
        try
        {
            _logger.LogInformation("Updating order in Cosmos DB: {OrderId}, Status: {Status}, type: {Type}",
                order.id,
                order.Status,
                order.type);

            await _container.ReplaceItemAsync(
                order,
                order.id,
                new PartitionKey(order.type));

            _logger.LogInformation("Order updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", order.id);
            throw;
        }
    }
}