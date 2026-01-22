namespace beaconinteriorsapi.Models
{
    public class ProductSearchParams
    {
        public required string? Name { get; set; }
        public required List<string> Category { get; set; } = new ();
    }
}
