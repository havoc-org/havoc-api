using Havoc_API.Models.DTOs.Role;
using Havoc_API.Models.DTOs.User;

namespace Havoc_API.Models.DTOs.Participation
{
    public class ParticipationGET
    {
        public int ProjectId { get;private set; }
        public virtual RoleGET Role { get; private set; } = null!;

        public virtual UserGET User { get; private set; } = null!;

        private ParticipationGET() { }
        public ParticipationGET(int projectId, RoleGET role, UserGET user)
        {
            ProjectId = projectId;
            this.Role = role;
            this.User = user;
        }
    }
}
