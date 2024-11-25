using AutoMapper;
using WidgetStore.Core.DTOs.Review;
using WidgetStore.Core.Entities;

namespace WidgetStore.Infrastructure.Mapping;

/// <summary>
/// AutoMapper profile for Review-related mappings
/// </summary>
public class ReviewMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the ReviewMappingProfile class
    /// </summary>
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.PartitionKey));

        CreateMap<CreateReviewDto, Review>();
    }
}