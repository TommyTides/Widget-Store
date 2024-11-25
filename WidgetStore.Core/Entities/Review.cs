using Azure;
using Azure.Data.Tables;

namespace WidgetStore.Core.Entities;

/// <summary>
/// Represents a product review in the system
/// </summary>
public class Review : ITableEntity
{
    /// <summary>
    /// Gets or sets the partition key (ProductId)
    /// </summary>
    public string PartitionKey { get; set; } = default!;

    /// <summary>
    /// Gets or sets the row key (ReviewId)
    /// </summary>
    public string RowKey { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the ETag
    /// </summary>
    public ETag ETag { get; set; }

    /// <summary>
    /// Gets or sets the review text
    /// </summary>
    public string Content { get; set; } = default!;

    /// <summary>
    /// Gets or sets the rating (1-5)
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the review date
    /// </summary>
    public DateTime ReviewDate { get; set; }
}