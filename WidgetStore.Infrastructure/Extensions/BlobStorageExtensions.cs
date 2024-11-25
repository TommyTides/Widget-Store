using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using WidgetStore.Core.Configuration;

namespace WidgetStore.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for configuring Azure Blob Storage
    /// </summary>
    public static class BlobStorageExtensions
    {
        /// <summary>
        /// Adds Azure Blob Storage services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="storageConfig">Storage configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddBlobStorage(
            this IServiceCollection services,
            AzureStorageConfig storageConfig)
        {
            // Add BlobServiceClient
            services.AddSingleton(x =>
                new BlobServiceClient(storageConfig.ConnectionString));

            // Add BlobContainerClient for product images
            services.AddSingleton(x =>
            {
                var blobServiceClient = x.GetRequiredService<BlobServiceClient>();
                return blobServiceClient.GetBlobContainerClient(
                    storageConfig.ProductImagesContainer);
            });

            return services;
        }

        /// <summary>
        /// Ensures that the blob container exists
        /// </summary>
        /// <param name="storageConfig">Storage configuration</param>
        public static async Task EnsureBlobContainerExistsAsync(
            AzureStorageConfig storageConfig)
        {
            var blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(
                storageConfig.ProductImagesContainer);

            await containerClient.CreateIfNotExistsAsync();

            // Set container to allow public access to blobs
            await containerClient.SetAccessPolicyAsync(
                Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }
    }
}