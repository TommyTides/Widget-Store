namespace WidgetStore.Core.DTOs.Order;

/// <summary>
/// Data transfer object for order validation in Azure Functions
/// </summary>
public class OrderValidationDto
{
    /// <summary>
    /// Gets or sets the order ID
    /// </summary>
    public string OrderId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the list of items to validate
    /// </summary>
    public List<OrderValidationItemDto> Items { get; set; } = new();
}

/// <summary>
/// Data transfer object for order item validation in Azure Functions
/// </summary>
public class OrderValidationItemDto
{
    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public string ProductId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the quantity to validate
    /// </summary>
    public int Quantity { get; set; }
}