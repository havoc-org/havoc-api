using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Services
{
    public interface IUserService
    {
        Task<bool> addUser(UserPOST user);
        Task<UserToken> verifyUser(UserLogin user);
        Task<UserGET> getUser(string email, string password);
        Task<bool> verifyEmail(string email);

    }
}
