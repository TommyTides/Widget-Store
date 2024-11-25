using WidgetStore.Core.Entities;

namespace WidgetStore.Core.Interfaces.Repositories;

/// <summary>
/// Interface for order repository operations
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Creates a new order in the database
    /// </summary>
    /// <param name="order">The order to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created order</returns>
    Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an order by its ID and user ID
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested order or null if not found</returns>
    Task<Order?> GetOrderAsync(string orderId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all orders for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of orders</returns>
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="order">The order to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated order</returns>
    Task<Order> UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves order metrics for analysis
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of orders with their metrics</returns>
    Task<IEnumerable<Order>> GetOrderMetricsAsync(CancellationToken cancellationToken = default);
}