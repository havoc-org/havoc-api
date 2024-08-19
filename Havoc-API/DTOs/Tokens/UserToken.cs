namespace Havoc_API.DTOs.Tokens
{
    public class UserToken
    {
        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public UserToken(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
