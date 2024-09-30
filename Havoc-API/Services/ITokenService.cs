using Havoc_API.DTOs.Tokens;
using Havoc_API.Models;
using System.Security.Claims;

namespace Havoc_API.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserToken user);
        string GenerateAccessToken(ClaimsPrincipal principal);
        string GenerateRefreshToken(int userId);
        ClaimsPrincipal ValidateRefreshToken(string token);

    }
}
