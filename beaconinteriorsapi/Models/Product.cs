using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace beaconinteriorsapi.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Summary { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Dimensions { get; set; }
        public List<Category> Categories { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public ICollection<Image> Images { get; set; } = [];
        public Product(string name, string description, decimal price, int quantity,  string? dimensions = null, string? summary = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Summary = summary;
            Description = description;
            Price = price;
            Quantity = quantity;
            Dimensions = dimensions;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
            Dimensions = dimensions;   
        }
        public Product()
        {
            
        }
    }
}
