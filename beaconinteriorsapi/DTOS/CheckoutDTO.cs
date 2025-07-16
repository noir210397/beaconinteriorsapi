using beaconinteriorsapi.Models;

namespace beaconinteriorsapi.DTOS
{
    public class CheckoutDTO
    {
       public required string? UserId { get; set; }
       public List<ItemDTO> Items { get; set; } = [];
       public required AddressDTO BillingAddress { get; set; }
       public AddressDTO? ShippingAddress { get; set; }
       public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }

    }
}
