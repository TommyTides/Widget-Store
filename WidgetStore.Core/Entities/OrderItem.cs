using System.Text.Json.Serialization;

namespace WidgetStore.Core.Entities;

/// <summary>
/// Represents an item within an order
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Gets or sets the ID of the product
    /// </summary>
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the quantity ordered
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price of the product at the time of order
    /// </summary>
    [JsonPropertyName("priceAtOrder")]
    public decimal PriceAtOrder { get; set; }
}