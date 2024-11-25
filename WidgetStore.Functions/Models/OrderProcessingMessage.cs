using System.Text.Json.Serialization;

namespace WidgetStore.Functions.Models;

/// <summary>
/// Represents a message for order processing
/// </summary>
public class OrderProcessingMessage
{
    /// <summary>
    /// Gets or sets the order ID
    /// </summary>
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the list of items to process
    /// </summary>
    [JsonPropertyName("items")]
    public List<OrderProcessingItem> Items { get; set; } = new();
}

/// <summary>
/// Represents an item in the order processing message
/// </summary>
public class OrderProcessingItem
{
    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the quantity ordered
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}