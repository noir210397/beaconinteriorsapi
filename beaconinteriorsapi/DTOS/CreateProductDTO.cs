using beaconinteriorsapi.Validators;
using System.ComponentModel.DataAnnotations;

namespace beaconinteriorsapi.DTOS
{
    public abstract class BaseCreateOrUpate
    {
        public required string Name { get; set; }
        public required string ShortDescription { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int Quantity { get; set; }
        public required string? Dimensions { get; set; }
        public required string Summary { get; set; }
        public required List<string> Categories { get; set; }
    }
    public class CreateProductDTO:BaseCreateOrUpate
    {
        public required  List<IFormFile> Images { get; set; }
    }
    public class UpdateProductDTO:BaseCreateOrUpate
    {
    public required List<string>? CurrentFiles { get; set; } 
    public required  List<string>? RemovedFiles { get; set; }
    public required List<IFormFile>? NewFiles { get; set; }
    }
}
