using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Services
{
    public interface IUserService
    {
        Task<bool> AddUserAsync(UserPOST user);
        Task<UserToken> VerifyUserAsync(UserLogin user);
        Task<UserGET> GetUserAsync(string email, string password);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> VerifyEmailAsync(string email);
        int GetUserId();

    }
}
