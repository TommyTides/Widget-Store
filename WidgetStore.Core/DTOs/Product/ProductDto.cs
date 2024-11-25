namespace WidgetStore.Core.DTOs.Product
{
    /// <summary>
    /// DTO for product information
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public string Id { get; set; } = string.Empty;  // This stays uppercase for DTO

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Current stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// URL to the product's image
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Category of the product
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// SKU (Stock Keeping Unit) of the product
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the product is available for sale
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public DateTime? ModifiedAt { get; set; }
    }
}