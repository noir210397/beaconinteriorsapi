namespace beaconinteriorsapi.Models
{
    public class Product
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<string> Images { get; set; }
        public Product(string? id,string name,string description,decimal price,int quantity,List<string> images)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Images = images;
        }
    }
}
