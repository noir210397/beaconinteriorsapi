using beaconinteriorsapi.Models;
using System.ComponentModel.DataAnnotations;

namespace beaconinteriorsapi.DTOS
{
    public class UserDTO
    {
        [Required]
        public required string Id { get; set; }
        [Required]
        [EmailAddress]
        public required string EmailAddress { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        public UserRoleType? RoleType { get; set; }
    }
}
