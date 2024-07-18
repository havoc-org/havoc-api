namespace Havoc_API.Models.DTOs.Participation
{
    public class NewProjectParticipationPOST
    {
        public int UserId { get;private set; }
        private NewProjectParticipationPOST() { }
        public NewProjectParticipationPOST( int userId)
        {
            this.UserId = userId;
        }
     
    }
}
