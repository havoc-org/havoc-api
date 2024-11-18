using Havoc_API.Models;

namespace Havoc_API.DTOs.Participation
{
    public class ParticipationPOST
    {
        public int ProjectId { get; private set; }

        public string Email { get; private set; } = null!;

        public RoleType Role { get; private set; }

        private ParticipationPOST() { }

        public ParticipationPOST(int projectId, string email, RoleType role)
        {
            ProjectId = projectId;
            Email = email;
            Role = role;
        }
    }
}
