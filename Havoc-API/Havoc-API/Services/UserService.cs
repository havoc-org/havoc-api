using Havoc_API.Data;
using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Havoc_API.Services
{
    public class UserService : IUserService
    {
        private readonly IHavocContext _context;
        private readonly IHttpContextAccessor _httpContextAccesor;
        public UserService(IHavocContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccesor = httpContextAccessor;
        }

        public async Task<bool> AddUserAsync(UserPOST user)
        {
            try
            {
                if (await VerifyEmailAsync(user.Email))
                    return false;

                var password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _context.Users.AddAsync(new Models.User(user.FirstName, user.LastName, user.Email, password));

                await _context.SaveChangesAsync();
                return true;
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
            catch (DbUpdateException e)
            {
                throw new DataAccessException(e.Message);
            }
        }
        public int GetUserId()
        {
            var token = _httpContextAccesor.HttpContext?.Items["JwtToken"]?.ToString();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Cannot find token");
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid or missing userId");
            }

            return userId;
        }

        public Task<UserGET> GetUserAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                    throw new NotFoundException("Cannot find user by Id " + userId);

                return user;
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
        }

        public async Task<bool> VerifyEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user != null;
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
        }

        public async Task<UserToken> VerifyUserAsync(UserLogin user)
        {
            try
            {
                var resultUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (resultUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, resultUser.Password))
                    throw new NotFoundException("Wrong email or password");
                else
                    return new UserToken(
                       resultUser.UserId,
                       resultUser.FirstName,
                       resultUser.LastName,
                       resultUser.Email
                    );
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
        }
    }
}
