namespace beaconinteriorsapi.Models
{
    public class Image
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string? PublicId { get; set; }
        public Guid ProductId { get; set; }
        public Image(string url, string? publicId=null)
        {
            Url = url;
            PublicId = publicId;
        }
    }
}
