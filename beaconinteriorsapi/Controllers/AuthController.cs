using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace beaconinteriorsapi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public AuthController(AuthService authService, ClaimsPrincipal claimsPrincipal)
        {
            _authService = authService;
            _claimsPrincipal= claimsPrincipal;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDTO value)
        {
            return Ok(await _authService.CreateUserAsync(value));
        }
        [HttpPost("admin")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDTO value)
        {
            return Ok(await _authService.CreateAdminAsync(value));
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO value)
        {           
            return Ok(await _authService.LogInAsync(value));
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO value)
        {
            //ensure that claims principal emailaddress is same as emailaddress
            var userEmailFromClaims = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            if (string.IsNullOrEmpty(userEmailFromClaims))
                return Unauthorized("No email claim found in token.");
            if (!string.Equals(userEmailFromClaims, value.EmailAddress, StringComparison.OrdinalIgnoreCase))
                return Forbid("Email mismatch: you can only change your own password.");
            await _authService.ChangePasswordAsync(value.EmailAddress, value.NewPassword);
            return NoContent();
        }
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO value)
        {
            return Ok(await _authService.ResetPasswordAsync(value.EmailAddress,value.ResetToken,value.NewPassword));
        }
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO value)
        {   
            var url = await _authService.ForgotPasswordAsync(value.EmailAddress);
            //send url for reset with email service
            return NoContent();
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO logoutRequest)
        {
            await _authService.LogOutAsync(logoutRequest.RefreshToken);
            return NoContent();
        }
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll ([FromBody] LogoutDTO logoutRequest)
        {
            await _authService.LogOutAllAsync(logoutRequest.RefreshToken);
            return NoContent();
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(LogoutDTO logoutRequest)
        {
            var tokens=await _authService.RefreshTokenAsync(logoutRequest.RefreshToken);
            return Ok(tokens);
        }
    }
}
