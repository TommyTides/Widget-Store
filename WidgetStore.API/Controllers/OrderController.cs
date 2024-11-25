using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WidgetStore.Core.DTOs.Order;
using WidgetStore.Core.Interfaces.Services;

namespace WidgetStore.API.Controllers;

/// <summary>
/// Controller for managing orders
/// </summary>
public class OrderController : BaseApiController
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    /// <summary>
    /// Initializes a new instance of the OrderController class
    /// </summary>
    /// <param name="orderService">The order service</param>
    /// <param name="logger">The logger</param>
    public OrderController(
        IOrderService orderService,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="createOrderDto">The order creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created order</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid input or insufficient stock</response>
    /// <response code="404">Product not found</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderDto createOrderDto,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.CreateOrderAsync(createOrderDto, userId, cancellationToken);

        return CreatedAtAction(
            nameof(GetOrder),
            new { orderId = order.Id },
            order);
    }

    /// <summary>
    /// Retrieves an order by ID
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested order</returns>
    /// <response code="200">Order retrieved successfully</response>
    /// <response code="404">Order not found</response>
    [HttpGet("{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetOrder(
    string orderId,
    CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.GetOrderAsync(orderId, userId, cancellationToken);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// Retrieves all orders for the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of orders</returns>
    /// <response code="200">Orders retrieved successfully</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);

        return Ok(orders);
    }

    /// <summary>
    /// Updates an existing order
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="updateOrderDto">The update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated order</returns>
    /// <response code="200">Order updated successfully</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Order not found</response>
    [HttpPut("{orderId}")]
    [Authorize]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> UpdateOrder(
        string orderId,
        [FromBody] UpdateOrderDto updateOrderDto,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.UpdateOrderAsync(orderId, updateOrderDto, userId, cancellationToken);

        return Ok(order);
    }

    /// <summary>
    /// Retrieves order metrics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order metrics report</returns>
    /// <response code="200">Metrics retrieved successfully</response>
    [HttpGet("metrics")]
    [Authorize(Roles = "Admin")] // Assuming only admins should see metrics
    [ProducesResponseType(typeof(OrderMetricsReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderMetricsReportDto>> GetOrderMetrics(
        CancellationToken cancellationToken)
    {
        var metrics = await _orderService.GetOrderMetricsAsync(cancellationToken);
        return Ok(metrics);
    }
}