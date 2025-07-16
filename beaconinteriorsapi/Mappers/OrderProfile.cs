using AutoMapper;
using beaconinteriorsapi.Models;

namespace beaconinteriorsapi.Mappers
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Product, OrderItems>()
                .ForMember(dest => dest.Quantity, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
