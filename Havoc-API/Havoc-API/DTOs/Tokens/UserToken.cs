namespace Havoc_API.DTOs.Tokens
{
    public class UserToken
    {
        public int Id { get; set; }
        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public UserToken(int UserId,string firstName, string lastName, string email)
        {
            Id= UserId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
