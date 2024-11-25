using AutoMapper;
using WidgetStore.Core.DTOs.Product;
using WidgetStore.Core.Entities;

namespace WidgetStore.Infrastructure.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.id, opt => opt.Ignore()) // Note the lowercase 'id'
                .ForMember(dest => dest.type, opt => opt.MapFrom(_ => "Product"))
                .ForMember(dest => dest.isAvailable, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.imageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.createdAt, opt => opt.Ignore())
                .ForMember(dest => dest.modifiedAt, opt => opt.Ignore());

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id));

            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.id, opt => opt.Ignore())
                .ForMember(dest => dest.type, opt => opt.Ignore())
                .ForMember(dest => dest.imageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.createdAt, opt => opt.Ignore())
                .ForMember(dest => dest.modifiedAt, opt => opt.Ignore());
        }
    }
}