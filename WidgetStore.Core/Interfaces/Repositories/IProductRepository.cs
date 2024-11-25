using WidgetStore.Core.Entities;

namespace WidgetStore.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for product operations
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="product">Product to create</param>
        /// <returns>Created product</returns>
        Task<Product> CreateAsync(Product product);

        /// <summary>
        /// Gets a product by its ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product if found, null otherwise</returns>
        Task<Product?> GetByIdAsync(string id);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>List of all products</returns>
        Task<IEnumerable<Product>> GetAllAsync();

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="product">Product to update</param>
        /// <returns>Updated product</returns>
        Task<Product> UpdateAsync(Product product);

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id">ID of the product to delete</param>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>List of products in the category</returns>
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    }
}
