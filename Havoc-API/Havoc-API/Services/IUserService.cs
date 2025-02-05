using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Models;

namespace Havoc_API.Services
{
    public interface IUserService
    {
        Task<bool> AddUserAsync(UserPOST user);
        Task<UserToken> VerifyUserAsync(UserLogin user);
        Task<UserGET> GetUserAsync(string email, string password);
        Task<User> GetUserByIdAsync(int userId);
        Task<UserGET> GetUserGETByIdAsync(int userId);
        Task<bool> VerifyEmailAsync(string email);
        Task<int> UpdateUserAsync(UserPATCH userPATCH);
        int GetUserId(HttpRequest request);

    }
}
