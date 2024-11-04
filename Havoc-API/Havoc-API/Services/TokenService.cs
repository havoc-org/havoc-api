using Azure.Core;
using Havoc_API.DTOs.Tokens;
using Havoc_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Havoc_API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GenerateToken(IEnumerable<Claim> claims, DateTime expireDate)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: expireDate,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateAccessToken(UserToken user)
        {
            var claims = new List<Claim>
             {
                new Claim("UserId",user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                 // Add more claims as necessary
              };

            return GenerateToken(claims, DateTime.UtcNow.AddHours(1));
        }

        public string GenerateAccessToken(ClaimsPrincipal principal)
        {
            var claims = principal.Claims.ToList();
            return GenerateToken(claims, DateTime.UtcNow.AddHours(1));
        }


        public string GenerateRefreshToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            var claims = new List<Claim>
            {
                new Claim("UserId",userId.ToString()),
                // Add more claims as necessary
            };

            var token = new JwtSecurityToken(
               issuer: _configuration["JWT:Issuer"],
               audience: _configuration["JWT:Audience"],
               claims: claims,
               expires: DateTime.UtcNow.AddDays(3),
               signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature));

            return tokenHandler.WriteToken(token);
        }




        public ClaimsPrincipal? ValidateRefreshToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return claimsPrincipal; // Если токен валиден, возвращаем его содержимое (Claims)
            }
            catch
            {
                return null; // Токен невалиден
            }
        }

    }
}
