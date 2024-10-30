using Havoc_API.DTOs.Participation;

namespace Havoc_API.Services
{
    public interface IParticipationService
    {
        public Task<ICollection<ParticipationGET>> GetParticipationsByProjectIDAsync(int projectId);
        public ICollection<ParticipationGET> GetParticipationsByUserID(string userId);
        public Task<bool> AddParticipationAsync(ParticipationPOST participation);
        public Task<bool> AddParticipationListAsync(List<ParticipationPOST> participationList);
    }
}
