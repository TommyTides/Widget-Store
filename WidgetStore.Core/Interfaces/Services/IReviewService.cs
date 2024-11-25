using WidgetStore.Core.DTOs.Review;

namespace WidgetStore.Core.Interfaces.Services;

/// <summary>
/// Service interface for review operations
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Creates a new anonymous review for a product
    /// </summary>
    /// <param name="productId">ID of the product being reviewed</param>
    /// <param name="reviewDto">Review creation data</param>
    /// <returns>Created review</returns>
    Task<ReviewDto> CreateReviewAsync(string productId, CreateReviewDto reviewDto);

    /// <summary>
    /// Gets all reviews for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>List of reviews</returns>
    Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(string productId);
}