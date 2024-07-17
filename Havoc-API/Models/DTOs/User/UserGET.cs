namespace Havoc_API.Models.DTOs.User
{
    public class UserGET
    {
        public int UserId { get;private set; }

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public string Password { get; private set; } = null!;

        public UserGET(int userId, string firstName, string lastName, string email, string password)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }
    }
}
