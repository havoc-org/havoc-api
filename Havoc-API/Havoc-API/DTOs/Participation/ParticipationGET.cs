using Havoc_API.DTOs.User;
using Havoc_API.DTOs.Role;

namespace Havoc_API.DTOs.Participation
{
    public class ParticipationGET
    {
        public int ProjectId { get; private set; }

        public virtual UserParticipationGET User { get; private set; } = null!;

        private ParticipationGET() { }
        public ParticipationGET(int projectId, UserParticipationGET user)
        {
            ProjectId = projectId;

            User = user;
        }
    }
}
