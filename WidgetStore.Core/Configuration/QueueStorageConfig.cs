namespace WidgetStore.Core.Configuration;

/// <summary>
/// Configuration for Azure Queue Storage
/// </summary>
public class QueueStorageConfig
{
    /// <summary>
    /// Gets or sets the connection string for Azure Storage
    /// </summary>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Gets or sets the name of the orders queue
    /// </summary>
    public string OrdersQueueName { get; set; } = default!;
}