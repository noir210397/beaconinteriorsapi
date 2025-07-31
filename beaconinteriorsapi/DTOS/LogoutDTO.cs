using System.ComponentModel.DataAnnotations;

namespace beaconinteriorsapi.DTOS
{
    public class LogoutDTO
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}
