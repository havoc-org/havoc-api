using Havoc_API.Models.DTOs.Participation;

namespace Havoc_API.Services
{
    public interface IParticipationService
    {
        public Task<ICollection<ParticipationGET>> GetParticipationsByProjectID(int projectId);
        public ICollection<ParticipationGET> GetParticipationsByUserID(string userId);
        public Task<bool> addParticipationAsync(ParticipationPOST participation);
    }
}
