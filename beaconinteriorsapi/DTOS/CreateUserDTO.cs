namespace beaconinteriorsapi.DTOS
{
    public class CreateUserDTO
    {
        public required string EmailAddress { get; set; }
        public required string Password { get; set; }
        public Boolean isAdmin { get; set; }
    }
}
