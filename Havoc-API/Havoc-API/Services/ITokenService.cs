using Havoc_API.DTOs.Tokens;
using Havoc_API.Models;
using System.Security.Claims;

namespace Havoc_API.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserToken user);

        string GenerateRefreshToken(int userId);
        ClaimsPrincipal? ValidateRefreshToken(string token);
        //Useless
        string GenerateAccessToken(ClaimsPrincipal principal);

    }
}
