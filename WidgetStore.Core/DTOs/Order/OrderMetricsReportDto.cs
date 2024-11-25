namespace WidgetStore.Core.DTOs.Order;

/// <summary>
/// Data transfer object for order metrics reporting
/// </summary>
public class OrderMetricsReportDto
{
    /// <summary>
    /// Gets or sets the average processing time in minutes
    /// </summary>
    public double AverageProcessingTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the average days from order to shipping
    /// </summary>
    public double AverageOrderToShipDays { get; set; }

    /// <summary>
    /// Gets or sets the total number of orders processed
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Gets or sets the total number of orders shipped
    /// </summary>
    public int TotalShippedOrders { get; set; }

    /// <summary>
    /// Gets or sets the total revenue from all orders
    /// </summary>
    public decimal TotalRevenue { get; set; }
}