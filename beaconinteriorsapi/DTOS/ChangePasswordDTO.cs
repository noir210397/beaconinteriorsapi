using System.ComponentModel.DataAnnotations;

namespace beaconinteriorsapi.DTOS
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public required string EmailAddress { get; set; }
    }
    public class ChangePasswordDTO:ForgotPasswordDTO
    {
        [Required]
        [MinLength(7)]
        public required string NewPassword { get; set; }
    }
    public class ResetPasswordDTO:ChangePasswordDTO
    {
        [Required]
        public required string ResetToken { get; set; }
    }
}
