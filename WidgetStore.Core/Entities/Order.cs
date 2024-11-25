using System.Text.Json.Serialization;

namespace WidgetStore.Core.Entities;

/// <summary>
/// Represents an order in the system
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique identifier for the order
    /// </summary>
    [JsonPropertyName("id")]
    public string id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the type of the document. Used for Cosmos DB partitioning
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; set; } = "Order";

    /// <summary>
    /// Gets or sets the ID of the user who placed the order
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the current status of the order
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the date when the order was placed
    /// </summary>
    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date when the order was shipped
    /// </summary>
    [JsonPropertyName("shippingDate")]
    public DateTime? ShippingDate { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the order
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the order items
    /// </summary>
    [JsonPropertyName("items")]
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the order metrics
    /// </summary>
    [JsonPropertyName("metrics")]
    public OrderMetrics? Metrics { get; set; }
}