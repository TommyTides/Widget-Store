using System.Text.Json.Serialization;

namespace WidgetStore.Core.Entities;

/// <summary>
/// Represents metrics associated with an order
/// </summary>
public class OrderMetrics
{
    /// <summary>
    /// Gets or sets the processing time in minutes
    /// </summary>
    [JsonPropertyName("processingTimeMinutes")]
    public int? ProcessingTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the number of days from order to shipping
    /// </summary>
    [JsonPropertyName("orderToShipDays")]
    public int? OrderToShipDays { get; set; }
}