using Havoc_API.Data;
using Havoc_API.DTOs.Project;
using Havoc_API.Models;
using Havoc_API.DTOs.Participation;
using Havoc_API.DTOs.ProjectStatus;
using Havoc_API.DTOs.Role;
using Havoc_API.DTOs.User;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Havoc_API.Exceptions;

namespace Havoc_API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IHavocContext _havocContext;
        private readonly IParticipationService _participationService;
        public ProjectService(IHavocContext context, IParticipationService participationService)
        {
            _havocContext = context;
            _participationService = participationService;
        }


        public async Task<int> AddProjectAsync(ProjectPOST project,User creator)
        {
            using (var transaction = _havocContext.Database.BeginTransaction())
            {
                
                var existingStatus = await _havocContext.ProjectStatuses.FirstOrDefaultAsync(st=>st.Name.Equals(project.ProjectStatus));
                if (creator == null)
                    throw new NotFoundException("Creator not found");

                ProjectStatus status = existingStatus == null ?  new ProjectStatus(project.ProjectStatus.Name) : existingStatus;
                
                if (existingStatus == null)
                await _havocContext.ProjectStatuses.AddAsync(status);

                Project newProject = new Project(
                    project.Name,
                    project.Description,
                    project.Background,
                    project.Start,
                    project.Deadline,
                    creator,
                    status
                    );
                await _havocContext.Projects.AddAsync(newProject);
                await _havocContext.SaveChangesAsync();

                project.Participations.Add(new NewProjectParticipationPOST(creator.Email));

                foreach (var par in project.Participations)
                {
                    var devRole = await _havocContext.Roles.Where(r => r.Name == "Developer").FirstAsync();
                    var user = await _havocContext.Users.Where(user=>user.Email==par.Email).FirstAsync();
                    if (user == null)
                        throw new NotFoundException("User for participation not found");

                    await _participationService.AddParticipationAsync(new ParticipationPOST(newProject.ProjectId,user.UserId,devRole.RoleId));
                }

                await _havocContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return newProject.ProjectId;
            }

        }

        public async Task<List<ProjectGET>> GetProjectsByUserAsync(int userId)
        {
            
            var project = await _havocContext.Projects.Where(p => p.Participations.Any(par => par.UserId == userId)).Select(project => new ProjectGET(
                project.ProjectId,
                project.Name,
                project.Description,
                project.Background,
                project.Start,
                project.Deadline,
                project.LastModified,
                new UserGET(
                    project.Creator.UserId,
                    project.Creator.FirstName,
                    project.Creator.LastName,
                    project.Creator.Email
                    ),
                new ProjectStatusGET(
                    project.ProjectStatus.ProjectStatusId,
                    project.ProjectStatus.Name
                    ),
                 _havocContext.Participations.Where(par => par.ProjectId == project.ProjectId)
                                             .Select(par => new ParticipationGET(
                    par.ProjectId,
                    new UserParticipationGET(
                        par.User.UserId,
                        par.User.FirstName,
                        par.User.LastName,
                        par.User.Email,
                        new RoleGET(
                        par.Role.RoleId,
                        par.Role.Name
                        )
                        )
                )).ToList()

            )).ToListAsync();
            return project;
        }

        public async Task<List<ProjectGET>> GetProjectsAsync()
        {
            
            var project = await _havocContext.Projects.Select(project => new ProjectGET(
                project.ProjectId,
                project.Name,
                project.Description,
                project.Background,
                project.Start,
                project.Deadline,
                project.LastModified,
                new UserGET(
                    project.Creator.UserId,
                    project.Creator.FirstName,
                    project.Creator.LastName,
                    project.Creator.Email
                    ),
                new ProjectStatusGET(
                    project.ProjectStatus.ProjectStatusId,
                    project.ProjectStatus.Name
                    ),
                 _havocContext.Participations.Where(par => par.ProjectId == project.ProjectId)
                                             .Select(par => new ParticipationGET(
                    par.ProjectId,
                    new UserParticipationGET(
                        par.User.UserId,
                        par.User.FirstName,
                        par.User.LastName,
                        par.User.Email,
                        new RoleGET(
                        par.Role.RoleId,
                        par.Role.Name
                        )
                        )
                )).ToList()
                
            )).ToListAsync();
            return project;
        }

    }
}
