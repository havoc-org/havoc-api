using Havoc_API.DTOs.Participation;
using Havoc_API.Models;

namespace Havoc_API.Services
{
    public interface IParticipationService
    {
        public Task<ICollection<ParticipationGET>> GetParticipationsByProjectIDAsync(int projectId);
        public ICollection<ParticipationGET> GetParticipationsByUserID(string userId);
        public Task<bool> AddParticipationAsync(ParticipationPOST participation);
        public Task<bool> AddParticipationListAsync(List<ParticipationPOST> participationList);
        public Task<Role> GetUserRoleInProjectAsync(int userId, int projectId);
        public Task<int> DeleteParticipation(int userId, int projectId);
    }
}
