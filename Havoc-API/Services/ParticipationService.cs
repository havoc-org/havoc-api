using Havoc_API.Data;
using Havoc_API.DTOs.Participation;
using Havoc_API.Models;
using Havoc_API.DTOs.Role;
using Havoc_API.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services
{
    public class ParticipationService : IParticipationService
    {
        private readonly IHavocContext _havocContext;
        public ParticipationService(IHavocContext havocContext)
        {
            _havocContext = havocContext;
        }


        public async Task<bool> AddParticipationAsync(ParticipationPOST participation)
        {
            var role = await _havocContext.Roles.Where(r => r.Name == "Developer").FirstAsync();
            if (role == null)
                throw new Exception("Role not found");

            var user = await _havocContext.Users.FindAsync(participation.UserId);
            if (user == null)
                throw new Exception("User not found");

            var existingParticipation = await _havocContext.Participations.FindAsync(participation.ProjectId, participation.UserId);
            if (existingParticipation != null)
                throw new Exception("This participation already exists userID: " + existingParticipation.UserId + " projectID: " + existingParticipation.ProjectId);
            var project = await _havocContext.Projects.FindAsync(participation.ProjectId);
            if (project == null)
                throw new Exception("Project not found");
            await _havocContext.Participations.AddAsync(new Participation(
                project,
                role,
                user
                ));
            return true;

        }

        public async Task<ICollection<ParticipationGET>> GetParticipationsByProjectIDAsync(int projectId)
        {
           return await _havocContext.Participations.Where(p => p.ProjectId == projectId)
                                             .Select(p => new ParticipationGET(
                    p.ProjectId,
                    new UserParticipationGET(
                        p.User.UserId,
                        p.User.FirstName,
                        p.User.LastName,
                        p.User.Email,
                        new RoleGET(
                        p.Role.RoleId,
                        p.Role.Name
                        )
                        )
                )).ToListAsync();
        }


        public ICollection<ParticipationGET> GetParticipationsByUserID(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
