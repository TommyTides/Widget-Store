using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.DTOs.Common;
using WidgetStore.Core.DTOs.Product;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.API.Controllers
{
    /// <summary>
    /// Controller for managing products
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IBlobStorageService _blobStorageService;

        /// <summary>
        /// Initializes a new instance of the ProductController
        /// </summary>
        /// <param name="productService">Product service instance</param>
        /// <param name="blobStorageService">Blob storage service instance</param>
        public ProductController(
            IProductService productService,
            IBlobStorageService blobStorageService)
        {
            _productService = productService;
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <returns>Created product details</returns>
        /// <response code="201">Returns the newly created product</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var product = await _productService.CreateAsync(createProductDto);
            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product);
        }

        /// <summary>
        /// Gets a product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        /// <response code="200">Returns the requested product</response>
        /// <response code="404">If the product is not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new ErrorResponse { Message = $"Product with ID {id} not found" });

            return Ok(product);
        }

        /// <summary>
        /// Gets all active products
        /// </summary>
        /// <returns>List of all active products</returns>
        /// <response code="200">Returns the list of products</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>List of products in the category</returns>
        /// <response code="200">Returns the list of products in the category</response>
        /// <response code="400">If the category is invalid</response>
        [HttpGet("category/{category}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _productService.GetByCategoryAsync(category);
            return Ok(products);
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Updated product data</param>
        /// <returns>Updated product details</returns>
        /// <response code="200">Returns the updated product</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        /// <response code="404">If the product is not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
        {
            var product = await _productService.UpdateAsync(id, updateProductDto);
            return Ok(product);
        }

        /// <summary>
        /// Deletes a product (soft delete)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content</returns>
        /// <response code="204">If the product was successfully deleted</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        /// <response code="404">If the product is not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Updates product stock quantity
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to add (positive) or remove (negative)</param>
        /// <returns>Updated product details</returns>
        /// <response code="200">Returns the updated product</response>
        /// <response code="400">If the quantity would make stock negative</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not authorized</response>
        /// <response code="404">If the product is not found</response>
        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStock(string id, [FromQuery] int quantity)
        {
            var product = await _productService.UpdateStockAsync(id, quantity);
            return Ok(product);
        }

        /// <summary>
        /// Uploads a product image
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="request">The image file to upload</param>
        /// <returns>Updated product details</returns>
        [HttpPost("{id}/image")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadProductImage(
            string id,
            [FromForm] ProductImageUploadDto request)
        {
            try
            {
                if (request?.File == null || request.File.Length == 0)
                    return BadRequest(new ErrorResponse { Message = "No file uploaded" });

                // Validate file size
                if (request.File.Length > BlobStorageConfig.MaxFileSizeInMB * 1024 * 1024)
                    return BadRequest(new ErrorResponse { Message = $"File size exceeds {BlobStorageConfig.MaxFileSizeInMB}MB limit" });

                // Validate content type
                if (!BlobStorageConfig.AllowedContentTypes.Contains(request.File.ContentType.ToLower()))
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid file type. Allowed types are: " +
                                 string.Join(", ", BlobStorageConfig.AllowedContentTypes)
                    });

                using var stream = request.File.OpenReadStream();
                var updatedProduct = await _productService.UploadImageAsync(
                    id,
                    request.File.FileName,
                    request.File.ContentType,
                    stream);

                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
        }
    }
}