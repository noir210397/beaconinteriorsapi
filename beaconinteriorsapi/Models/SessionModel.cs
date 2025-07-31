namespace beaconinteriorsapi.Models
{
    public class SessionModel
    {
        public Guid Id { get; set; }
        public required string UserIdentifier { get; set; }
        public required string RefreshTokenID    { get; set; }
    }
}
