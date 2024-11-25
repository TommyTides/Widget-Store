using Microsoft.AspNetCore.Mvc;
using WidgetStore.Core.DTOs.Review;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.API.Controllers;

/// <summary>
/// Controller for managing product reviews
/// </summary>
public class ReviewController : BaseApiController
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewController> _logger;

    /// <summary>
    /// Initializes a new instance of the ReviewController class
    /// </summary>
    /// <param name="reviewService">Review service</param>
    /// <param name="logger">Logger instance</param>
    public ReviewController(
        IReviewService reviewService,
        ILogger<ReviewController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new review for a product
    /// </summary>
    /// <param name="productId">The product ID</param>
    /// <param name="createReviewDto">The review creation data</param>
    /// <returns>The created review</returns>
    /// <response code="201">Review created successfully</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Product not found</response>
    [HttpPost("products/{productId}/reviews")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> CreateReview(
        string productId,
        [FromBody] CreateReviewDto createReviewDto)
    {
        var review = await _reviewService.CreateReviewAsync(productId, createReviewDto);

        return CreatedAtAction(
            nameof(GetProductReviews),
            new { productId },
            review);
    }

    /// <summary>
    /// Retrieves all reviews for a specific product
    /// </summary>
    /// <param name="productId">The product ID</param>
    /// <returns>Collection of reviews</returns>
    /// <response code="200">Reviews retrieved successfully</response>
    /// <response code="404">Product not found</response>
    [HttpGet("products/{productId}/reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetProductReviews(string productId)
    {
        var reviews = await _reviewService.GetProductReviewsAsync(productId);
        return Ok(reviews);
    }
}