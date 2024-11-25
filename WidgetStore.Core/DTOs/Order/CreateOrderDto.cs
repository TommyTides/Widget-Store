using System.ComponentModel.DataAnnotations;

namespace WidgetStore.Core.DTOs.Order;

/// <summary>
/// Data transfer object for creating a new order
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// Gets or sets the list of items in the order
    /// </summary>
    [Required]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// Data transfer object for creating a new order item
/// </summary>
public class CreateOrderItemDto
{
    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    [Required]
    public string ProductId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the quantity of the product
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}