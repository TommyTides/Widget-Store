using WidgetStore.Core.DTOs.Product;

namespace WidgetStore.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for image operations
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Uploads a product image to blob storage
        /// </summary>
        /// <param name="productId">ID of the product</param>
        /// <param name="imageStream">Image file stream</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="contentType">Content type of the file</param>
        /// <returns>Image details including URL</returns>
        Task<ProductImageDto> UploadProductImageAsync(string productId, Stream imageStream, string fileName, string contentType);

        /// <summary>
        /// Deletes a product image from blob storage
        /// </summary>
        /// <param name="imageUrl">URL of the image to delete</param>
        Task DeleteProductImageAsync(string imageUrl);
    }
}