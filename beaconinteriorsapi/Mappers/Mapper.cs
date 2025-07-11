using AutoMapper;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.Utils;

namespace beaconinteriorsapi.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Entity to DTO
            CreateMap<Product, ProductDTO>()
                //.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.Url)))
                .ForMember(dest=>dest.Id,opt=>opt.MapFrom(src=>GuidUtils.ToGuidString(src.Id)))
                .ReverseMap()
                //ignore to prevent duplicate categories in DB
                //.ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ForMember(dest=>dest.Id,opt=>opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<CreateProductDTO, Product>().ForMember(dest=>dest.Images,opt=>opt.Ignore()).ForMember(dest => dest.Categories, opt => opt.Ignore());
            CreateMap<UpdateProductDTO, Product>().ForMember(dest => dest.Categories, opt => opt.Ignore());


        }
    }
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }
    }

}
