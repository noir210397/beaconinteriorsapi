using beaconinteriorsapi.Models;

namespace beaconinteriorsapi.DTOS
{
    public class AddressDTO
    {
        public required string StreetAddress { get; set; }
        public required string City { get; set; }
        public required string PostCode { get; set; }
        public required string Country { get; set; }
    }
}
