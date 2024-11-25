namespace WidgetStore.Core.DTOs.Product
{
    /// <summary>
    /// DTO for updating an existing product
    /// </summary>
    public class UpdateProductDto
    {
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
    }
}