using Havoc_API.Models;
using Havoc_API.Models.DTOs.Project;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Services
{
    public interface IProjectService
    {
        public Task<int> addProject(ProjectPOST project);
        public Task<List<ProjectGET>> getProjects();
    }
}
