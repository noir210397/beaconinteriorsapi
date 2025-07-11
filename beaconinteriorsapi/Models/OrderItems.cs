namespace beaconinteriorsapi.Models
{
    public class OrderItems
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string? Summary { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Dimensions { get; set; }
        public required string Categories { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public Guid OrderId { get; set; }
        public required string  Images { get; set; }
        public OrderItems()
        {
            
        }
    }
}
