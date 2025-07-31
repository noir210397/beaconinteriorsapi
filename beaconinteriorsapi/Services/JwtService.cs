using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using beaconinteriorsapi.Models;
using beaconinteriorsapi.DTOS;
using beaconinteriorsapi.Data;
namespace beaconinteriorsapi.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration, BeaconInteriorsDBContext dbContext)
        {
            _configuration = configuration;
        }


        public JwtTokenDTO GenerateTokens(User user, IList<string> roles, Guid refreshTokenId)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            var refreshKey = Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET_KEY");

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(refreshKey))
                throw new Exception("JWT keys are missing in environment variables");

            var accessToken = GenerateAccessToken(user, roles, jwtKey);
            var refreshToken = GenerateRefreshToken(user, refreshKey, refreshTokenId);

            return new JwtTokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateAccessToken(User user, IList<string> roles, string key)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Email, user.UserName!),
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(User user, string key, Guid refreshTokenId)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, refreshTokenId.ToString()),
                new Claim("type", "refresh")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ClaimsPrincipal? ValidateRefreshToken(string refreshToken)
        {
            var key = Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET_KEY");
            if (string.IsNullOrEmpty(key)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = true,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, validationParams, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Claims.Any(c => c.Type == "type" && c.Value == "refresh"))
                    return null;
                return principal;
            }
            catch
            {
                return null;
            }
        }



    }
}
