using Microsoft.AspNetCore.Identity;
namespace beaconinteriorsapi.Models
{
    public class User:IdentityUser
    {
       public required string FirstName { get; set; }
       public required string LastName { get; set; }
       public bool MustChangePassWord { get; set; }=false;
    }
}
