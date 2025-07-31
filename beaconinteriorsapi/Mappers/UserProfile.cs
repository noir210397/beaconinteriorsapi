using AutoMapper;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
namespace beaconinteriorsapi.Mappers
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<User,UserDTO>().
                ReverseMap();
        }
    }
}
