using AutoMapper;
using WidgetStore.Core.DTOs.Order;
using WidgetStore.Core.Entities;

namespace WidgetStore.Infrastructure.Mapping;

/// <summary>
/// AutoMapper profile for Order-related mappings
/// </summary>
public class OrderMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the OrderMappingProfile class
    /// </summary>
    public OrderMappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<OrderMetrics, OrderMetricsDto>();

        // DTO to Entity mappings
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.type, opt => opt.MapFrom(src => "Order"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<CreateOrderItemDto, OrderItem>();

        CreateMap<UpdateOrderDto, Order>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Function DTOs mappings
        CreateMap<OrderValidationDto, Order>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<OrderValidationItemDto, OrderItem>();

        // Metrics mapping
        CreateMap<IEnumerable<Order>, OrderMetricsReportDto>()
            .ForMember(dest => dest.TotalOrders, opt =>
                opt.MapFrom(src => src.Count()))
            .ForMember(dest => dest.TotalShippedOrders, opt =>
                opt.MapFrom(src => src.Count(o => o.ShippingDate.HasValue)))
            .ForMember(dest => dest.TotalRevenue, opt =>
                opt.MapFrom(src => src.Sum(o => o.TotalAmount)))
            .ForMember(dest => dest.AverageProcessingTimeMinutes, opt =>
                opt.MapFrom(src =>
                    src.Where(o => o.Metrics != null && o.Metrics.ProcessingTimeMinutes.HasValue)
                       .Select(o => o.Metrics.ProcessingTimeMinutes.Value)
                       .DefaultIfEmpty()
                       .Average()))
            .ForMember(dest => dest.AverageOrderToShipDays, opt =>
                opt.MapFrom(src =>
                    src.Where(o => o.Metrics != null && o.Metrics.OrderToShipDays.HasValue)
                       .Select(o => o.Metrics.OrderToShipDays.Value)
                       .DefaultIfEmpty()
                       .Average()));
    }
}