using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.Infrastructure.Services;

/// <summary>
/// Implementation of queue service using Azure Storage Queues
/// </summary>
public class QueueService : IQueueService
{
    private readonly QueueStorageConfig _config;

    /// <summary>
    /// Initializes a new instance of the QueueService class
    /// </summary>
    /// <param name="config">Queue storage configuration</param>
    public QueueService(IOptions<QueueStorageConfig> config)
    {
        _config = config.Value;
    }

    /// <inheritdoc/>
    public async Task SendMessageAsync(string queueName, string message, CancellationToken cancellationToken = default)
    {
        var queueClient = new QueueClient(
            _config.ConnectionString,
            queueName,
            new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await queueClient.SendMessageAsync(message, cancellationToken: cancellationToken);
    }
}