using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for blob storage operations
    /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;

        /// <summary>
        /// Initializes a new instance of BlobStorageService
        /// </summary>
        /// <param name="blobServiceClient">Blob service client</param>
        /// <param name="logger">Logger instance</param>
        public BlobStorageService(
            BlobServiceClient blobServiceClient,
            ILogger<BlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<string> UploadAsync(
            string containerName,
            string fileName,
            string contentType,
            Stream stream)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new BadRequestException("File name is required");

                if (stream == null || stream.Length == 0)
                    throw new BadRequestException("File content is required");

                // Validate file size
                if (stream.Length > BlobStorageConfig.MaxFileSizeInMB * 1024 * 1024)
                    throw new BadRequestException($"File size exceeds {BlobStorageConfig.MaxFileSizeInMB}MB limit");

                // Validate content type
                if (!BlobStorageConfig.AllowedContentTypes.Contains(contentType.ToLower()))
                    throw new BadRequestException("Invalid file type. Allowed types are: " +
                        string.Join(", ", BlobStorageConfig.AllowedContentTypes));

                // Get container client
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();

                // Generate unique file name
                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                // Set blob HTTP headers
                var headers = new BlobHttpHeaders
                {
                    ContentType = contentType,
                    CacheControl = "public, max-age=31536000"
                };

                // Upload the file
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = headers
                });

                return blobClient.Uri.ToString();
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error uploading blob: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to upload file to blob storage", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                await blobClient.DeleteIfExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error deleting blob: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to delete file from blob storage", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Stream> GetAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                    throw new NotFoundException($"File {fileName} not found");

                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error downloading blob: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to download file from blob storage", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                return await blobClient.ExistsAsync();
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error checking blob existence: {Message}", ex.Message);
                throw new InvalidOperationException("Failed to check file existence in blob storage", ex);
            }
        }

        /// <summary>
        /// Extracts the file name from a blob URL
        /// </summary>
        /// <param name="blobUrl">URL of the blob</param>
        /// <returns>File name</returns>
        private static string GetFileNameFromUrl(string blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl))
                return string.Empty;

            return Path.GetFileName(new Uri(blobUrl).LocalPath);
        }
    }
}