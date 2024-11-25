using Microsoft.Azure.Cosmos;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Interfaces.Repositories;

namespace WidgetStore.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Order-related operations using Cosmos DB
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the OrderRepository
    /// </summary>
    /// <param name="container">Cosmos DB container instance</param>
    public OrderRepository(Container container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    /// <inheritdoc/>
    public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            order,
            new PartitionKey(order.type),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<Order?> GetOrderAsync(string orderId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<Order>(
                id: orderId,
                partitionKey: new PartitionKey("Order"),
                cancellationToken: cancellationToken);

            var order = response.Resource;

            return order;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'Order' AND c.UserId = @userId ORDER BY c.OrderDate DESC")
            .WithParameter("@userId", userId);

        var iterator = _container.GetItemQueryIterator<Order>(query);
        var results = new List<Order>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<Order> UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var response = await _container.UpsertItemAsync(
            order,
            new PartitionKey(order.type),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetOrderMetricsAsync(CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.type = 'Order' AND IS_DEFINED(c.metrics)");

        var iterator = _container.GetItemQueryIterator<Order>(query);
        var results = new List<Order>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }
}