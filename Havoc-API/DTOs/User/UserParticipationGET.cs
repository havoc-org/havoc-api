using Havoc_API.DTOs.Role;

namespace Havoc_API.DTOs.User
{
    public class UserParticipationGET
    {
        public int UserId { get; private set; }

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;
        public virtual RoleGET Role { get; private set; } = null!;

        public UserParticipationGET(int userId, string firstName, string lastName, string email, RoleGET role)
        {
            Role = role;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
