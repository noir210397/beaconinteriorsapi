using beaconinteriorsapi.Data;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

namespace beaconinteriorsapi.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtService _jwtService;
        private readonly BeaconInteriorsDBContext _dbContext;


        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, JwtService jwtService, BeaconInteriorsDBContext dbContext) {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _dbContext = dbContext;
        }
        public async Task<JwtTokenDTO> CreateUserAsync(RegisterDTO value)
        {
            var userExists = await _userManager.FindByEmailAsync(value.Email);
            if (userExists != null) {
                ThrowBadRequest($"user with email:{value.Email} already exists");
            }
            IdentityResult result = await _userManager.CreateAsync(new User() { FirstName = value.FirstName, LastName = value.LastName, Email = value.Email, UserName = value.Email },value.Password);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(value.Email);
                var roleResult = await _userManager.AddToRoleAsync(user!, UserRoleType.User.ToString());
                if (roleResult.Succeeded)
                {
                    var refreshTokenId=Guid.NewGuid();
                    var tokens = _jwtService.GenerateTokens(user!, new List<string> { UserRoleType.User.ToString() },refreshTokenId);
                    await _dbContext.Sessions.AddAsync(new SessionModel() { RefreshTokenID = refreshTokenId.ToString(), UserIdentifier = user!.Id });
                    return tokens;
                }
                else
                {
                    //var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    //throw new Exception($"unable to create user: {errors}");
                    throw new Exception("unable to add roles to user");
                }

            }
            else
            {
                //var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                //throw new Exception($"unable to create user: {errors}");
                throw new Exception("unable to create user");
            }
        }
    
        public async Task<LoginDTO> CreateAdminAsync(RegisterDTO value)
        {
            var adminExists = _userManager.FindByEmailAsync(value.Email);
            if (adminExists != null)
            {
                ThrowBadRequest($"user with email:{value.Email} already exists");
            }
            IdentityResult result = await _userManager.CreateAsync(new User() { FirstName = value.FirstName, LastName = value.LastName, Email = value.Email, UserName = value.Email, PasswordHash = value.Password,MustChangePassWord=true });
            if (result.Succeeded)
            {
                var admin = await _userManager.FindByEmailAsync(value.Email);
                var roleResult = await _userManager.AddToRoleAsync(admin!, UserRoleType.Admin.ToString());
                if (roleResult.Succeeded)
                {
                    return new LoginDTO() { Email=value.Email,Password=value.Password};
                }
                else { throw new Exception("unable to add roles to admin"); }

            }
            else { throw new Exception("unable to create admin"); }
        }
        public async Task<JwtTokenDTO> LogInAsync(LoginDTO values)
        {
            var user =await _userManager.FindByEmailAsync(values.Email.Trim());
            if (user == null)
            {
                ThrowBadRequest($"no user found by email: {values.Email}");
            }
            var signInResult=await _userManager.CheckPasswordAsync(user!,values.Password);
            if (!signInResult) { 
                ThrowBadRequest("invalid password try again");
            }
            if (user!.MustChangePassWord)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(resetToken); // Make it URL-safe
                ThrowUnauthorizedError($"you are required to change your password||{encodedToken}");
            }
            var roles = await _userManager.GetRolesAsync(user!);
            var refreshTokenId = Guid.NewGuid();
            await _dbContext.Sessions.AddAsync(new SessionModel() { RefreshTokenID = refreshTokenId.ToString(), UserIdentifier = user!.Id });
            var tokens = _jwtService.GenerateTokens(user!, roles,refreshTokenId);
            return tokens;
        }
        public async Task LogOutAsync(string refreshToken)
        {
            var principal = _jwtService.ValidateRefreshToken(refreshToken);
            if (principal == null)
                ThrowUnauthorizedError("Invalid refresh token");

            var refreshTokenId = principal!.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrEmpty(refreshTokenId))
                ThrowUnauthorizedError("Invalid token payload");

            var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenID == refreshTokenId);
            if (session == null)
                ThrowUnauthorizedError("Session not found or already logged out");

            _dbContext.Sessions.Remove(session!);
            await _dbContext.SaveChangesAsync();
        }
        public async Task LogOutAllAsync(string refreshToken)
        {
            var principal = _jwtService.ValidateRefreshToken(refreshToken);
            if (principal == null)
                ThrowUnauthorizedError("Invalid refresh token");

            var userId = principal!.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
                ThrowUnauthorizedError("Invalid token payload");

            var sessions = await _dbContext.Sessions.Where(s => s.UserIdentifier == userId).ToListAsync();
            if (sessions.Any())
            {
                _dbContext.Sessions.RemoveRange(sessions);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<JwtTokenDTO> RefreshTokenAsync(string refreshToken)
        {
            var principal = _jwtService.ValidateRefreshToken(refreshToken);
            if (principal == null)
                ThrowUnauthorizedError("Invalid refresh token");
            var refreshTokenId = principal!.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var userId = principal!.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var session=await _dbContext.Sessions.FirstOrDefaultAsync(s=>s.RefreshTokenID == refreshTokenId);
            if (string.IsNullOrEmpty(refreshTokenId)|| string.IsNullOrEmpty(userId)||session==null) ThrowUnauthorizedError("invalid token payload");
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
                ThrowUnauthorizedError("User not found");
            Guid newRefreshTokenId=Guid.NewGuid();
            session!.RefreshTokenID=newRefreshTokenId.ToString();
            var roles = await _userManager.GetRolesAsync(user!);
            var tokens=   _jwtService.GenerateTokens(user!, roles,newRefreshTokenId);
            await _dbContext.SaveChangesAsync();
            return tokens;
        }
        public async Task ChangePasswordAsync(string email,string newPassword)
        {
            var user=await _userManager.FindByEmailAsync(email);
            if(user==null) ThrowBadRequest($"no user found with email {email}");
            var token=await _userManager.GeneratePasswordResetTokenAsync(user!);
            await _userManager.ResetPasswordAsync(user!,token,newPassword);
        }
        public async Task<JwtTokenDTO> ResetPasswordAsync(string email,string token,string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) ThrowBadRequest($"no user found with email {email}");
            var decodedToken=WebUtility.UrlDecode(token);
            bool isValid = await _userManager.VerifyUserTokenAsync(user!, TokenOptions.DefaultProvider, "ResetPassword", decodedToken);
            if (!isValid) ThrowBadRequest("invalid reset token provided");
            await _userManager.ResetPasswordAsync(user!,decodedToken,newPassword);
            user!.MustChangePassWord = true;
            await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user!);
            var refreshTokenId = Guid.NewGuid();
            await _dbContext.Sessions.AddAsync(new SessionModel() { RefreshTokenID = refreshTokenId.ToString(), UserIdentifier = user!.Id });
            var tokens = _jwtService.GenerateTokens(user!, roles, refreshTokenId);
            return tokens;

        }
        public async Task<string> ForgotPasswordAsync(string value)
        {
            var user = await _userManager.FindByEmailAsync(value.Trim());
            if (user == null)
                ThrowNotFound($"No user found with email {value}");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user!);
            var encodedToken = WebUtility.UrlEncode(token);
            return $"?email={user!.Email}&token={encodedToken}";
        }


    }
}
