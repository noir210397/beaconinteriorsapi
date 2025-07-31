using System.ComponentModel.DataAnnotations;

namespace beaconinteriorsapi.DTOS
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "First Name is required")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public required string LastName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
