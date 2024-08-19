using Havoc_API.Data;
using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services
{
    public class UserService : IUserService
    {
        private readonly IHavocContext _context;
        public UserService(IHavocContext context)
        {
            _context = context;
        }

        public async Task<bool> addUser(UserPOST user)
        {
            if (await verifyEmail(user.Email))
                return false;

            var password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _context.Users.AddAsync(new Models.User(user.FirstName,user.LastName,user.Email,password));

            await _context.SaveChangesAsync();
            return true;
        }

        public Task<UserGET> getUser(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> verifyEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user == null ? false : true;
        }

        public async Task<UserToken> verifyUser(UserLogin user)
        {
            var resultUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (resultUser == null || !BCrypt.Net.BCrypt.Verify(user.Password,resultUser.Password ))
                throw new NotFoundException("Wrong email or password");
            else
                return new UserToken(
                   resultUser.FirstName,
                   resultUser.LastName,
                   resultUser.Email
                );
        }
    }
}
