using Havoc_API.DTOs.Project;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Services
{
    public interface IProjectService
    {
        public Task<int> addProjectAsync(ProjectPOST project);
        public Task<List<ProjectGET>> getProjectsAsync();
        Task<List<ProjectGET>> getProjectsByUser(int userId);
    }
}
