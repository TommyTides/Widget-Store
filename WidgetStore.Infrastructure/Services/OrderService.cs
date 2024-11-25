using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.DTOs.Order;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Repositories;
using WidgetStore.Core.Interfaces.Services;
using WidgetStore.Core.Messages;
using Microsoft.Extensions.Options;


namespace WidgetStore.Infrastructure.Services;

/// <summary>
/// Service implementation for order-related operations
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IQueueService _queueService;
    private readonly QueueStorageConfig _queueConfig;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    /// <summary>
    /// Initializes a new instance of the OrderService class
    /// </summary>
    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IQueueService queueService,
        IOptions<QueueStorageConfig> queueConfig,
        IMapper mapper,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _queueService = queueService;
        _queueConfig = queueConfig.Value;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new order for user {UserId}", userId);

        var order = _mapper.Map<Order>(orderDto);
        order.UserId = userId;

        _logger.LogInformation("Validating products and calculating total amount");
        // Calculate prices and validate stock
        decimal totalAmount = 0;
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);

            if (product == null)
            {
                _logger.LogError("Product not found: {ProductId}", item.ProductId);
                throw new NotFoundException($"Product {item.ProductId} not found");
            }

            if (!product.isAvailable)
            {
                _logger.LogError("Product not available: {ProductId}", item.ProductId);
                throw new BadRequestException($"Product {product.name} is not available for purchase");
            }

            item.PriceAtOrder = product.price;
            totalAmount += item.PriceAtOrder * item.Quantity;
            _logger.LogInformation("Added item to order: ProductId: {ProductId}, Quantity: {Quantity}, Price: {Price}",
                item.ProductId, item.Quantity, item.PriceAtOrder);
        }

        order.TotalAmount = totalAmount;
        _logger.LogInformation("Order total amount calculated: {TotalAmount}", totalAmount);

        // Create the order
        _logger.LogInformation("Creating order in database");
        var createdOrder = await _orderRepository.CreateOrderAsync(order, cancellationToken);
        _logger.LogInformation("Order created successfully: {OrderId}", createdOrder.id);

        // Create and send queue message
        var queueMessage = new OrderProcessingMessage
        {
            OrderId = createdOrder.id,
            UserId = createdOrder.UserId,
            Items = createdOrder.Items.Select(item => new OrderProcessingItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        var messageJson = JsonSerializer.Serialize(queueMessage);

        await _queueService.SendMessageAsync(
            _queueConfig.OrdersQueueName,
            messageJson,
            cancellationToken);

        _logger.LogInformation("Message enqueued successfully");

        return _mapper.Map<OrderDto>(createdOrder);
    }

    /// <inheritdoc/>
    public async Task<OrderDto?> GetOrderAsync(string orderId, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving order {OrderId} for user {UserId}", orderId, userId);

        var order = await _orderRepository.GetOrderAsync(orderId, userId, cancellationToken);

        if (order == null)
        {
            _logger.LogInformation("Order {OrderId} not found or doesn't belong to user {UserId}", orderId, userId);
            return null;
        }

        return _mapper.Map<OrderDto>(order);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all orders for user {UserId}", userId);

        var orders = await _orderRepository.GetUserOrdersAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    /// <inheritdoc/>
    public async Task<OrderDto> UpdateOrderAsync(string orderId, UpdateOrderDto updateOrderDto, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating order {OrderId} for user {UserId}", orderId, userId);

        var existingOrder = await _orderRepository.GetOrderAsync(orderId, userId, cancellationToken)
            ?? throw new NotFoundException($"Order {orderId} not found");

        // Update only the status and shipping date
        existingOrder.Status = updateOrderDto.Status;
        if (updateOrderDto.ShippingDate.HasValue)
        {
            existingOrder.ShippingDate = updateOrderDto.ShippingDate;

            // Calculate metrics when shipping date is set
            if (existingOrder.Metrics == null)
            {
                existingOrder.Metrics = new OrderMetrics();
            }
            existingOrder.Metrics.OrderToShipDays = (int)(updateOrderDto.ShippingDate.Value - existingOrder.OrderDate).TotalDays;
        }

        var updatedOrder = await _orderRepository.UpdateOrderAsync(existingOrder, cancellationToken);
        return _mapper.Map<OrderDto>(updatedOrder);
    }

    /// <inheritdoc/>
    public async Task<OrderMetricsReportDto> GetOrderMetricsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving order metrics");

        var orders = await _orderRepository.GetOrderMetricsAsync(cancellationToken);
        return _mapper.Map<OrderMetricsReportDto>(orders);
    }
}