using beaconinteriorsapi.Models;

namespace beaconinteriorsapi.Commands
{
    public class ItemCommand
    {
        public required Guid Id { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckoutCommand
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string? UserId { get; set; }
        public List<ItemCommand> Items { get; set; } = [];
        public required string PhoneNumber { get; set; }
        public required string EmailAddress { get; set; }
        public required Address BillingAddress { get; set; }
        public required Address ShippingAddress { get; set; }
    }


}
