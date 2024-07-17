    using Havoc_API.Data;
    using Havoc_API.Models;
    using Havoc_API.Models.DTOs.Project;
using Havoc_API.Models.DTOs.ProjectStatus;
using Havoc_API.Models.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services
    {
        public class ProjectService : IProjectService
        {
            private readonly IHavocContext _havocContext;
            public ProjectService(IHavocContext context) {
                 _havocContext = context;
            }


            public async Task<int> addProject(ProjectPOST project) 
            {

            using (var transaction = _havocContext.Database.BeginTransaction())
            {
                var creator = await _havocContext.Users.FindAsync(project.CreatorId);
                if (creator == null)
                    throw new Exception("User not found");
                ProjectStatus status = new ProjectStatus(project.ProjectStatus.Name);
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
                await transaction.CommitAsync();
                return newProject.ProjectId;
            }
                
            }
            public async Task<List<ProjectGET>> getProjects()
            {
                var project= await _havocContext.Projects.Select(o=>new ProjectGET(
                    o.ProjectId,
                    o.Name,
                    o.Description,
                    o.Background,
                    o.Start,
                    o.Deadline,
                    o.LastModified,
                    new UserGET(
                        o.Creator.UserId,
                        o.Creator.FirstName,
                        o.Creator.LastName,
                        o.Creator.Email,
                        o.Creator.Password
                        ),
                    new ProjectStatusGET(
                        o.ProjectStatus.ProjectStatusId,
                        o.ProjectStatus.Name)
                    )).ToListAsync();
                return project;
            }

        }
    }
