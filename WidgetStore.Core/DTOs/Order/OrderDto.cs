namespace WidgetStore.Core.DTOs.Order;

/// <summary>
/// Data transfer object for order information
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Gets or sets the order ID
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user ID who placed the order
    /// </summary>
    public string UserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the order status
    /// </summary>
    public string Status { get; set; } = default!;

    /// <summary>
    /// Gets or sets the order date
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Gets or sets the shipping date
    /// </summary>
    public DateTime? ShippingDate { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the order
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the list of items in the order
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the order metrics
    /// </summary>
    public OrderMetricsDto? Metrics { get; set; }
}

/// <summary>
/// Data transfer object for order item information
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public string ProductId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the quantity ordered
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price at the time of order
    /// </summary>
    public decimal PriceAtOrder { get; set; }
}

/// <summary>
/// Data transfer object for order metrics information
/// </summary>
public class OrderMetricsDto
{
    /// <summary>
    /// Gets or sets the processing time in minutes
    /// </summary>
    public int? ProcessingTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the days from order to shipping
    /// </summary>
    public int? OrderToShipDays { get; set; }
}