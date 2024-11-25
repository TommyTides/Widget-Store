using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Interfaces.Repositories;

namespace WidgetStore.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for review operations using Table Storage
/// </summary>
public class ReviewRepository : IReviewRepository
{
    private readonly TableClient _tableClient;

    /// <summary>
    /// Initializes a new instance of the ReviewRepository class
    /// </summary>
    /// <param name="storageConfig">Storage configuration</param>
    public ReviewRepository(IOptions<AzureStorageConfig> storageConfig)
    {
        var tableServiceClient = new TableServiceClient(storageConfig.Value.ConnectionString);
        _tableClient = tableServiceClient.GetTableClient(storageConfig.Value.ReviewsTableName);
        _tableClient.CreateIfNotExists();
    }

    /// <inheritdoc/>
    public async Task<Review> CreateAsync(Review review)
    {
        await _tableClient.AddEntityAsync(review);
        return review;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Review>> GetByProductIdAsync(string productId)
    {
        var reviews = _tableClient.QueryAsync<Review>(r => r.PartitionKey == productId);
        var reviewsList = new List<Review>();

        await foreach (var review in reviews)
        {
            reviewsList.Add(review);
        }

        return reviewsList;
    }
}