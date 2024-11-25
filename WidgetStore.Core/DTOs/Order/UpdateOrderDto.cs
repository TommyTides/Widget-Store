using System.ComponentModel.DataAnnotations;

namespace WidgetStore.Core.DTOs.Order;

/// <summary>
/// Data transfer object for updating an order
/// </summary>
public class UpdateOrderDto
{
    /// <summary>
    /// Gets or sets the order status
    /// </summary>
    [Required]
    public string Status { get; set; } = default!;

    /// <summary>
    /// Gets or sets the shipping date
    /// </summary>
    public DateTime? ShippingDate { get; set; }
}