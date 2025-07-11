namespace beaconinteriorsapi.Models
{
    public class Category
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        //ensures many to many relationship table between Products and Category is created
        public List<Product> Products { get; set; } = [];
        public Category(string name)
        {
            Name= name;
        }
        public Category()
        {
            
        }
    }
}
