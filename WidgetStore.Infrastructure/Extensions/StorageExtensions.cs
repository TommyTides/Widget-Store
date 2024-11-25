using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using WidgetStore.Core.Configuration;

namespace WidgetStore.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for configuring Azure Storage services
    /// </summary>
    public static class StorageExtensions
    {
        /// <summary>
        /// Adds Azure Storage services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="config">Azure storage configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddAzureStorage(
            this IServiceCollection services,
            AzureStorageConfig config)
        {
            // Add Table Client for Users
            services.AddSingleton(x =>
                new TableClient(
                    config.ConnectionString,
                    config.UsersTableName));

            // Add Table Client for Reviews
            services.AddSingleton(x =>
                new TableClient(
                    config.ConnectionString,
                    config.ReviewsTableName));

            // Add Blob Service Client for Images
            services.AddSingleton(x =>
                new BlobServiceClient(config.ConnectionString));

            // Add Blob Container Client for Product Images
            services.AddSingleton(x =>
            {
                var blobServiceClient = x.GetRequiredService<BlobServiceClient>();
                return blobServiceClient.GetBlobContainerClient(config.ProductImagesContainer);
            });

            return services;
        }

        /// <summary>
        /// Ensures that all required Azure Storage resources exist
        /// </summary>
        /// <param name="config">Azure storage configuration</param>
        /// <returns>Task representing the initialization process</returns>
        public static async Task EnsureStorageResourcesExistAsync(AzureStorageConfig config)
        {
            // Create table client for Users
            var usersTableClient = new TableClient(
                config.ConnectionString,
                config.UsersTableName);
            await usersTableClient.CreateIfNotExistsAsync();

            // Create table client for Reviews
            var reviewsTableClient = new TableClient(
                config.ConnectionString,
                config.ReviewsTableName);
            await reviewsTableClient.CreateIfNotExistsAsync();

            // Create blob container for product images
            var blobServiceClient = new BlobServiceClient(config.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(
                config.ProductImagesContainer);
            await containerClient.CreateIfNotExistsAsync();
        }
    }
}
