namespace WidgetStore.Core.Interfaces.Services;

/// <summary>
/// Service for interacting with Azure Storage Queues
/// </summary>
public interface IQueueService
{
    /// <summary>
    /// Sends a message to a queue
    /// </summary>
    /// <param name="queueName">Name of the queue</param>
    /// <param name="message">Message to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendMessageAsync(string queueName, string message, CancellationToken cancellationToken = default);
}