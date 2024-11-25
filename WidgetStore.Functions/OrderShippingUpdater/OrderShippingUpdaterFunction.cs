using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using WidgetStore.Core.Entities;

namespace WidgetStore.Functions.OrderShippingUpdater;

/// <summary>
/// Azure Function for updating order shipping status
/// </summary>
public class OrderShippingUpdaterFunction
{
    private readonly Container _container;
    private readonly ILogger<OrderShippingUpdaterFunction> _logger;

    /// <summary>
    /// Initializes a new instance of the OrderShippingUpdaterFunction class
    /// </summary>
    /// <param name="container">Cosmos DB container</param>
    /// <param name="logger">Logger instance</param>
    public OrderShippingUpdaterFunction(
        Container container,
        ILogger<OrderShippingUpdaterFunction> logger)
    {
        _container = container;
        _logger = logger;
    }

    /// <summary>
    /// Processes orders ready for shipping
    /// Runs every 5 minutes
    /// </summary>
    [Function("OrderShippingUpdater")]
    public async Task Run([TimerTrigger("0 0 9 * * *")] FunctionContext context) // Runs every minute for proof of concept sake.
    {
        try
        {
            _logger.LogInformation("Starting order shipping update process at: {time}", DateTime.UtcNow);

            // Query for processed orders that haven't been shipped
            var query = new QueryDefinition(
                @"SELECT * FROM c 
                  WHERE c.type = 'Order' 
                  AND c.status = 'Processed' 
                  AND (NOT IS_DEFINED(c.shippingDate) OR c.shippingDate = null)");

            var iterator = _container.GetItemQueryIterator<Order>(query);
            var processedCount = 0;

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var order in response)
                {
                    try
                    {
                        // Update order with shipping information
                        order.Status = "Shipped";
                        order.ShippingDate = DateTime.UtcNow;

                        // Update metrics
                        if (order.Metrics == null)
                        {
                            order.Metrics = new OrderMetrics();
                        }

                        order.Metrics.OrderToShipDays =
                            (int)(order.ShippingDate.Value - order.OrderDate).TotalDays;

                        await _container.UpsertItemAsync(
                            order,
                            new PartitionKey(order.type));

                        processedCount++;
                        _logger.LogInformation(
                            "Updated shipping status for order {OrderId}",
                            order.id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error updating shipping status for order {OrderId}",
                            order.id);
                    }
                }
            }

            _logger.LogInformation(
                "Completed order shipping update process. Processed {Count} orders",
                processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error in order shipping update process");
            throw;
        }
    }
}