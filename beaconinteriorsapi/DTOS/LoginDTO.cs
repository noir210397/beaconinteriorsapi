using System.ComponentModel.DataAnnotations;

namespace beaconinteriorsapi.DTOS
{
    public class LoginDTO
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
