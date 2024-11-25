using AutoMapper;
using Microsoft.Extensions.Logging;
using WidgetStore.Core.DTOs.Review;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Repositories;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.Infrastructure.Services;

/// <summary>
/// Service implementation for review operations
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewService> _logger;

    /// <summary>
    /// Initializes a new instance of the ReviewService class
    /// </summary>
    /// <param name="reviewRepository">Review repository</param>
    /// <param name="productRepository">Product repository</param>
    /// <param name="mapper">AutoMapper instance</param>
    /// <param name="logger">Logger instance</param>
    public ReviewService(
        IReviewRepository reviewRepository,
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<ReviewService> logger)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ReviewDto> CreateReviewAsync(string productId, CreateReviewDto reviewDto)
    {
        _logger.LogInformation("Creating new review for product {ProductId}", productId);

        // Verify product exists
        var product = await _productRepository.GetByIdAsync(productId)
            ?? throw new NotFoundException($"Product {productId} not found");

        // Create review entity
        var review = new Review
        {
            PartitionKey = productId,  // ProductId as partition key
            RowKey = Guid.NewGuid().ToString(),  // Unique review ID
            Content = reviewDto.Content,
            Rating = reviewDto.Rating,
            ReviewDate = DateTime.UtcNow,
            Timestamp = DateTimeOffset.UtcNow
        };

        // Save review
        await _reviewRepository.CreateAsync(review);

        // Map to DTO
        return new ReviewDto
        {
            Id = review.RowKey,
            ProductId = review.PartitionKey,
            Content = review.Content,
            Rating = review.Rating,
            ReviewDate = review.ReviewDate
        };
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(string productId)
    {
        _logger.LogInformation("Retrieving reviews for product {ProductId}", productId);

        // Verify product exists
        var product = await _productRepository.GetByIdAsync(productId)
            ?? throw new NotFoundException($"Product {productId} not found");

        // Get reviews
        var reviews = await _reviewRepository.GetByProductIdAsync(productId);

        // Map to DTOs
        return reviews.Select(review => new ReviewDto
        {
            Id = review.RowKey,
            ProductId = review.PartitionKey,
            Content = review.Content,
            Rating = review.Rating,
            ReviewDate = review.ReviewDate
        });
    }
}