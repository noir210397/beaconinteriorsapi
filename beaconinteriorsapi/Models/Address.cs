namespace beaconinteriorsapi.Models
{
    public class Address
    {
        public  long Id { get; set; }
        public required string  StreetAddress    { get; set; }
        public required string City { get; set; }
        public required string PostCode  { get; set; }
        public required string Country { get; set; }
        public AddressType AddressType { get; set; }
        public Guid OrderId { get; set; }
        public Address()
        {
            
        }
    }
}
