using System.ComponentModel.DataAnnotations;

namespace WidgetStore.Core.DTOs.Review;

/// <summary>
/// Data transfer object for creating a review
/// </summary>
public class CreateReviewDto
{
    /// <summary>
    /// Gets or sets the review content
    /// </summary>
    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Content { get; set; } = default!;

    /// <summary>
    /// Gets or sets the rating
    /// </summary>
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}
