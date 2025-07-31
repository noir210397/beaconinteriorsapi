using AutoMapper;
using beaconinteriorsapi.Commands;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
namespace beaconinteriorsapi.Mappers
{

    public class CheckoutProfile : Profile
    {
        public CheckoutProfile()
        {
            CreateMap<CheckoutDTO, CheckoutCommand>()
                .ForMember(dest => dest.UserId,
                           opt => opt.MapFrom(src =>
                               string.IsNullOrWhiteSpace(src.UserId)
                                   ? null
                                   : src.UserId))
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.Items));

            CreateMap<ItemDTO, ItemCommand>()
                .ForMember(dest => dest.Id,
                           opt => opt.MapFrom(src => Guid.Parse(src.Id!)));
            CreateMap<AddressDTO, Address>();
        }
    }

}
