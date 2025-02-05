using Havoc_API.DTOs.Role;
using System.Data;

namespace Havoc_API.DTOs.User
{
    public class UserGET
    {
        public int UserId { get; private set; }

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;
        public int AssignmentCount { get; set; }
        public int ParticipationCount { get; set; }

        public UserGET(int userId, string firstName, string lastName, string email)
        {

            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
