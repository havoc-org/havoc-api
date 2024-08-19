using Havoc_API.DTOs.Tokens;
using Havoc_API.Models;

namespace Havoc_API.Services
{
    public interface ITokenService
    {
        string GenerateToken(UserToken user);


    }
}
