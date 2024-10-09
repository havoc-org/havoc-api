using Havoc_API.DTOs.Project;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Services
{
    public interface IProjectService
    {
        public Task<int> AddProjectAsync(ProjectPOST project);
        public Task<List<ProjectGET>> GetProjectsAsync();
        Task<List<ProjectGET>> GetProjectsByUserAsync(int userId);
    }
}
