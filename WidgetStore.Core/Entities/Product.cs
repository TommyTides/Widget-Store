using System.Text.Json.Serialization;

namespace WidgetStore.Core.Entities
{
    /// <summary>
    /// Represents a product in the system
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        [JsonPropertyName("id")]
        public string id { get; set; } = string.Empty;  // Note: lowercase 'id' for Cosmos DB

        /// <summary>
        /// Type that defines the partition key for Cosmos DB
        /// </summary>
        [JsonPropertyName("type")]
        public string type { get; set; } = "Product";

        /// <summary>
        /// Name of the product
        /// </summary>
        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the product
        /// </summary>
        [JsonPropertyName("description")]
        public string description { get; set; } = string.Empty;

        /// <summary>
        /// Price of the product
        /// </summary>
        [JsonPropertyName("price")]
        public decimal price { get; set; }

        /// <summary>
        /// Current stock quantity
        /// </summary>
        [JsonPropertyName("stockQuantity")]
        public int stockQuantity { get; set; }

        /// <summary>
        /// URL to the product's image in blob storage
        /// </summary>
        [JsonPropertyName("imageUrl")]
        public string? imageUrl { get; set; }

        /// <summary>
        /// Category of the product
        /// </summary>
        [JsonPropertyName("category")]
        public string category { get; set; } = string.Empty;

        /// <summary>
        /// SKU (Stock Keeping Unit) of the product
        /// </summary>
        [JsonPropertyName("sku")]
        public string sku { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the product is available for sale
        /// </summary>
        [JsonPropertyName("isAvailable")]
        public bool isAvailable { get; set; } = true;

        /// <summary>
        /// Timestamp when the entity was created
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the entity was last modified
        /// </summary>
        [JsonPropertyName("modifiedAt")]
        public DateTime? modifiedAt { get; set; }
    }
}