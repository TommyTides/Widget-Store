using WidgetStore.Core.Entities;

namespace WidgetStore.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for review operations
/// </summary>
public interface IReviewRepository
{
    /// <summary>
    /// Creates a new review
    /// </summary>
    /// <param name="review">Review to create</param>
    /// <returns>Created review</returns>
    Task<Review> CreateAsync(Review review);

    /// <summary>
    /// Gets all reviews for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>List of reviews</returns>
    Task<IEnumerable<Review>> GetByProductIdAsync(string productId);
}