using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using WidgetStore.Core.Configuration;

namespace WidgetStore.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for configuring Cosmos DB services
    /// </summary>
    public static class CosmosDbExtensions
    {
        /// <summary>
        /// Adds Cosmos DB services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="config">Cosmos DB configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddCosmosDb(
            this IServiceCollection services,
            CosmosDbConfig config)
        {
            // Validate configuration
            if (string.IsNullOrEmpty(config.DatabaseName))
                throw new ArgumentException("Database name cannot be null or empty", nameof(config));
            if (string.IsNullOrEmpty(config.ContainerName))
                throw new ArgumentException("Container name cannot be null or empty", nameof(config));

            // Add CosmosClient as singleton with explicit serializer settings
            services.AddSingleton(sp =>
            {
                var cosmosClientOptions = new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                        IgnoreNullValues = true
                    }
                };

                return new CosmosClient(config.ConnectionString, cosmosClientOptions);
            });

            // Create database and container first
            using (var client = new CosmosClient(config.ConnectionString))
            {
                client.CreateDatabaseIfNotExistsAsync(config.DatabaseName).GetAwaiter().GetResult();
                var database = client.GetDatabase(config.DatabaseName);

                var containerProperties = new ContainerProperties(config.ContainerName, "/type");
                database.CreateContainerIfNotExistsAsync(containerProperties, 400).GetAwaiter().GetResult();
            }

            // Register CosmosConfig
            services.AddSingleton(config);

            // Register Container directly
            services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<CosmosClient>();
                var database = client.GetDatabase(config.DatabaseName);
                return database.GetContainer(config.ContainerName);
            });

            return services;
        }

        /// <summary>
        /// Ensures that the Cosmos DB database and container exist
        /// </summary>
        /// <param name="config">Cosmos DB configuration</param>
        public static async Task EnsureCosmosDbResourcesExistAsync(CosmosDbConfig config)
        {
            try
            {
                using var client = new CosmosClient(config.ConnectionString);

                // Create database if it doesn't exist
                var database = await client.CreateDatabaseIfNotExistsAsync(config.DatabaseName);

                // Configure container properties
                var containerProperties = new ContainerProperties
                {
                    Id = config.ContainerName,
                    PartitionKeyPath = "/type",
                    IndexingPolicy = new IndexingPolicy
                    {
                        IndexingMode = IndexingMode.Consistent,
                        Automatic = true,
                        IncludedPaths =
                        {
                            new IncludedPath { Path = "/*" }
                        }
                    }
                };

                // Create container if it doesn't exist
                await database.Database.CreateContainerIfNotExistsAsync(
                    containerProperties,
                    throughput: 400);

                Console.WriteLine($"Ensured database {config.DatabaseName} and container {config.ContainerName} exist");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring Cosmos DB resources exist: {ex.Message}");
                throw;
            }
        }
    }
}