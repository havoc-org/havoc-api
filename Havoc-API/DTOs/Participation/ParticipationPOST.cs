namespace Havoc_API.DTOs.Participation
{
    public class ParticipationPOST
    {
        public int ProjectId { get; private set; }

        public string Email { get; private set; }

        public string Role { get; private set; }

        private ParticipationPOST() { }

        public ParticipationPOST(int projectId, string email, string role)
        {
            ProjectId = projectId;
            Email = email;
            Role = role;

        }
    }
}
