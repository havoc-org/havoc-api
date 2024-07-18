namespace Havoc_API.Models.DTOs.Participation
{
    public class ParticipationPOST
    {
        public int ProjectId { get; private set; }

        public int UserId { get; private set; }

        public int RoleId { get; private set; }

        private ParticipationPOST() { }

        public ParticipationPOST(int projectId, int userId, int roleId)
        {
            ProjectId = projectId;
            UserId = userId;
            RoleId = roleId;
            
        }
    }
}
