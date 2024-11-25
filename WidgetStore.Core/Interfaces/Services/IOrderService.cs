using WidgetStore.Core.DTOs.Order;

namespace WidgetStore.Core.Interfaces.Services;

/// <summary>
/// Interface for order-related business operations
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="orderDto">The order creation data</param>
    /// <param name="userId">The ID of the user creating the order</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created order</returns>
    Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an order by its ID
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested order or null if not found</returns>
    Task<OrderDto?> GetOrderAsync(string orderId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all orders for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of orders</returns>
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="updateOrderDto">The update data</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated order</returns>
    Task<OrderDto> UpdateOrderAsync(string orderId, UpdateOrderDto updateOrderDto, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves order metrics for analysis
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order metrics report</returns>
    Task<OrderMetricsReportDto> GetOrderMetricsAsync(CancellationToken cancellationToken = default);
}