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
                var role = await _havocContext.Roles.Where(r => r.Name.Equals(participation.Role)).FirstOrDefaultAsync() 
                ?? throw new NotFoundException("Cannot find Role: " + participation.Role);

                var user = await _havocContext.Users.FirstOrDefaultAsync(us => us.Email.Equals(participation.Email)) 
                ?? throw new NotFoundException("User not found");

                var existingParticipation = await _havocContext.Participations.FindAsync(participation.ProjectId, user.UserId);
                if (existingParticipation != null)
                    throw new InvalidOperationException("This participation already exists userID: " + existingParticipation.UserId + " projectID: " + existingParticipation.ProjectId);

                var project = await _havocContext.Projects.FindAsync(participation.ProjectId) 
                ?? throw new NotFoundException("Project not found");
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

        public async Task<ParticipationGET> PatchParticipantRoleAsync(int userId, int projectId, ParticipationPATCH patch)
        {
            try
            {
                if (!Enum.TryParse<RoleType>(patch.Role, true, out var parsedRole))
                {
                    throw new NotFoundException("Invalid role: " + patch.Role);
                }

                var participation = await _havocContext.Participations
                    .Include(p => p.Role)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.ProjectId == projectId);

                if (participation == null)
                    throw new NotFoundException("Participation not found");

                var role = await _havocContext.Roles.FirstOrDefaultAsync(r => r.Name == parsedRole) 
                ?? throw new NotFoundException("Role not found");
                participation.updateParticipationRole(role);
                await _havocContext.SaveChangesAsync();

                return new ParticipationGET(
                    participation.ProjectId,
                    new UserParticipationGET(
                        participation.User.UserId,
                        participation.User.FirstName,
                        participation.User.LastName,
                        participation.User.Email,
                        new RoleGET(role.RoleId, role.Name)
                    )
                );
            }
            catch (SqlException e)
            {
                throw new DataAccessException(e.Message);
            }
            catch (DbUpdateException e)
            {
                throw new DataAccessException(e.Message);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex.Message);
            }
        }

        public async Task<int> DeleteParticipation(int userId, int projectId)
        {
            try
            {
                var participation = await _havocContext.Participations
                        .Include(a => a.User)
                        .FirstOrDefaultAsync(a => a.UserId == userId && a.ProjectId == projectId) 
                        ?? throw new NotFoundException($"Participation for UserId={userId}, ProjectId={projectId} doesn't exist.");
                _havocContext.Participations.Remove(participation);
                return await _havocContext.SaveChangesAsync();
            }
            catch (SqlException e)
            {
                Console.WriteLine($"[Error] Database error occurred: {e.Message}");
                throw new DataAccessException($"Database error: {e.Message}");
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine($"[Error] Database update error occurred: {e.Message}");
                throw new DataAccessException($"Database update error: {e.Message}");
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

        public async Task<int> AddUserToProjectThroughInviteCodeAsync(int userId, string inviteCode)
        {
            var decryptedInviteCode = Project.DecryptInviteCode(inviteCode);

            var project = await _havocContext.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == int.Parse(decryptedInviteCode["ProjectId"])
                && p.Name == decryptedInviteCode["ProjectName"]) ?? throw new NotFoundException("Project Not Found");

            var user = await _havocContext.Users.FindAsync(userId) ?? throw new NotFoundException("User Not Found");

            bool isUserAlreadyInProject = await _havocContext.Participations
            .AnyAsync(p => p.ProjectId == project.ProjectId && p.User.UserId == userId);

            if (isUserAlreadyInProject)
            {
                throw new InvalidOperationException("User is already a participant in this project.");
            }

            var newParticipation = new Participation(project, new Role(RoleType.Developer), user);

            await _havocContext.Participations.AddAsync(newParticipation);
            await _havocContext.SaveChangesAsync();

            return newParticipation.ProjectId;
        }
    }
}
