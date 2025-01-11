using Havoc_API.Data;
using Havoc_API.DTOs.Participation;
using Havoc_API.Models;
using Havoc_API.DTOs.Role;
using Havoc_API.DTOs.User;
using Microsoft.EntityFrameworkCore;
using Havoc_API.Exceptions;
using Microsoft.Data.SqlClient;

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
            try
            {
                var role = await _havocContext.Roles.Where(r => r.Name.Equals(participation.Role)).FirstOrDefaultAsync();
                if (role == null)
                    throw new NotFoundException("Cannot find Role: " + participation.Role);

                var user = await _havocContext.Users.FirstOrDefaultAsync(us => us.Email.Equals(participation.Email));
                if (user == null)
                {
                    return false;   // change when need to implement if there is no participant in database

                    throw new Exception("User not found");
                }
                var existingParticipation = await _havocContext.Participations.FindAsync(participation.ProjectId, user.UserId);
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
                await _havocContext.SaveChangesAsync();
                return true;
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
            catch (DbUpdateException e)
            {
                throw new DataAccessException(e.Message);
            }

        }

        public async Task<bool> AddParticipationListAsync(List<ParticipationPOST> participationList)
        {
            try
            {
                foreach (var par in participationList)
                    await AddParticipationAsync(new ParticipationPOST(par.ProjectId, par.Email, par.Role));
                return true;
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
            catch (DbUpdateException e)
            {
                throw new DataAccessException(e.Message);
            }
        }


        public async Task<ICollection<ParticipationGET>> GetParticipationsByProjectIDAsync(int projectId)
        {
            try
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
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
        }


        public ICollection<ParticipationGET> GetParticipationsByUserID(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> GetUserRoleInProjectAsync(int userId, int projectId)
        {
            try
            {
                var participation = await _havocContext.Participations
                    .Include(p => p.Role)
                    .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.UserId == userId)
                    ?? throw new NotFoundException("Participation not found");

                return participation.Role;
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
        }
    }
}
