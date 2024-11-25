using WidgetStore.Core.DTOs.Product;

namespace WidgetStore.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for product operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <returns>Created product details</returns>
        /// <exception cref="BadRequestException">Thrown when product data is invalid</exception>
        Task<ProductDto> CreateAsync(CreateProductDto createProductDto);

        /// <summary>
        /// Gets a product by its ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details if found, null otherwise</returns>
        Task<ProductDto?> GetByIdAsync(string id);

        /// <summary>
        /// Gets all active products
        /// </summary>
        /// <returns>List of all active products</returns>
        Task<IEnumerable<ProductDto>> GetAllAsync();

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Updated product data</param>
        /// <param name="imageUrl">Optional new image URL</param>
        /// <returns>Updated product details</returns>
        /// <exception cref="BadRequestException">Thrown when product data is invalid</exception>
        /// <exception cref="NotFoundException">Thrown when product is not found</exception>
        Task<ProductDto> UpdateAsync(string id, UpdateProductDto updateProductDto, string? imageUrl = null);

        /// <summary>
        /// Soft deletes a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <exception cref="NotFoundException">Thrown when product is not found</exception>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>List of products in the category</returns>
        /// <exception cref="BadRequestException">Thrown when category is invalid</exception>
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);

        /// <summary>
        /// Updates product stock
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to add (positive) or remove (negative)</param>
        /// <returns>Updated product details</returns>
        /// <exception cref="NotFoundException">Thrown when product is not found</exception>
        /// <exception cref="BadRequestException">Thrown when stock would become negative</exception>
        Task<ProductDto> UpdateStockAsync(string id, int quantity);

        /// <summary>
        /// Uploads a product image
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="fileName">Name of the image file</param>
        /// <param name="contentType">Content type of the image</param>
        /// <param name="imageStream">Image content stream</param>
        /// <returns>Updated product details</returns>
        /// <exception cref="NotFoundException">Thrown when product is not found</exception>
        /// <exception cref="BadRequestException">Thrown when image is invalid</exception>
        Task<ProductDto> UploadImageAsync(string id, string fileName, string contentType, Stream imageStream);
    }
}