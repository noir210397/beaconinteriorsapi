using System.ComponentModel.DataAnnotations;

namespace beaconinteriorsapi.DTOS
{
    public class ProductDTO
    {
        //omit all properties included in product model you dont want sent to the client.
        public  string? Id { get; set; }
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Dimensions { get; set; }=null;
        public required List<CategoryDTO> Categories { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime LastUpdatedAt { get; set; }
        public required List<string> Images { get; set; }
    }
}
