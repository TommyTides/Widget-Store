using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using WidgetStore.Core.Configuration;
using WidgetStore.Core.Entities;
using WidgetStore.Core.Exceptions;
using WidgetStore.Core.Interfaces.Repositories;

namespace WidgetStore.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for product operations using Cosmos DB
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly Container _container;

        /// <summary>
        /// Initializes a new instance of ProductRepository
        /// </summary>
        /// <param name="container">Cosmos DB container for products</param>
        public ProductRepository(Container container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc/>
        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                var response = await _container.CreateItemAsync(
                    item: product,
                    partitionKey: new PartitionKey(product.type));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw new BadRequestException("A product with this ID already exists.");
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Product>(
                    id: id,
                    partitionKey: new PartitionKey("Product"));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"Product with ID {id} not found.");
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.type = 'Product'");
            var iterator = _container.GetItemQueryIterator<Product>(queryDefinition);

            var results = new List<Product>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        /// <inheritdoc/>
        public async Task<Product?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Product>(
                    id: id,
                    partitionKey: new PartitionKey("Product"));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            var queryDefinition = new QueryDefinition(
                "SELECT * FROM c WHERE c.type = 'Product' AND c.category = @category")
                .WithParameter("@category", category);

            var iterator = _container.GetItemQueryIterator<Product>(queryDefinition);

            var results = new List<Product>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        /// <inheritdoc/>
        public async Task<Product> UpdateAsync(Product product)
        {
            try
            {
                var response = await _container.UpsertItemAsync(
                    item: product,
                    partitionKey: new PartitionKey(product.type));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"Product with ID {product.id} not found.");
            }
        }
    }
}