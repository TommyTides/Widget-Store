using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.DTOs.Product;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for image operations
    /// </summary>
    public class ImageService : IImageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string[] _allowedContentTypes = { "image/jpeg", "image/png", "image/gif" };
        private const int MaxFileSizeInMB = 5;

        /// <summary>
        /// Initializes a new instance of the ImageService
        /// </summary>
        /// <param name="containerClient">Blob container client</param>
        public ImageService(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        /// <inheritdoc/>
        public async Task<ProductImageDto> UploadProductImageAsync(
            string productId,
            Stream imageStream,
            string fileName,
            string contentType)
        {
            // Validate content type
            if (!_allowedContentTypes.Contains(contentType.ToLower()))
            {
                throw new BadRequestException(
                    $"Invalid file type. Allowed types are: {string.Join(", ", _allowedContentTypes)}");
            }

            // Validate file size
            if (imageStream.Length > MaxFileSizeInMB * 1024 * 1024)
            {
                throw new BadRequestException(
                    $"File size exceeds {MaxFileSizeInMB}MB limit.");
            }

            try
            {
                // Generate unique blob name
                var extension = Path.GetExtension(fileName);
                var blobName = $"{productId}/{Guid.NewGuid()}{extension}";
                var blobClient = _containerClient.GetBlobClient(blobName);

                // Upload the file
                await blobClient.UploadAsync(
                    imageStream,
                    new BlobHttpHeaders
                    {
                        ContentType = contentType
                    });

                return new ProductImageDto
                {
                    ImageUrl = blobClient.Uri.ToString(),
                    FileName = fileName,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to upload image to blob storage.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteProductImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return;

                // Extract blob name from URL
                var uri = new Uri(imageUrl);
                var blobName = uri.Segments[^1];
                var blobClient = _containerClient.GetBlobClient(blobName);

                // Delete the blob
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to delete image from blob storage.", ex);
            }
        }
    }
}