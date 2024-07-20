namespace Havoc_API.DTOs.Participation
{
    public class NewProjectParticipationPOST
    {
        public int UserId { get; private set; }
        private NewProjectParticipationPOST() { }
        public NewProjectParticipationPOST(int userId)
        {
            UserId = userId;
        }

    }
}
