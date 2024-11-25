namespace WidgetStore.Core.DTOs.Review;

/// <summary>
/// Data transfer object for review information
/// </summary>
public class ReviewDto
{
    /// <summary>
    /// Gets or sets the review ID
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public string ProductId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the review content
    /// </summary>
    public string Content { get; set; } = default!;

    /// <summary>
    /// Gets or sets the rating
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the review date
    /// </summary>
    public DateTime ReviewDate { get; set; }
}