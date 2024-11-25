namespace WidgetStore.Core.DTOs.Product
{
    /// <summary>
    /// DTO for creating a new product
    /// </summary>
    public class CreateProductDto
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
        /// Initial stock quantity
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
    }
}