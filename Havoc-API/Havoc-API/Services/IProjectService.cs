﻿using Havoc_API.DTOs.Project;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Services
{
    public interface IProjectService
    {
        public Task<int> AddProjectAsync(ProjectPOST project, User creator);
        public Task<IEnumerable<ProjectGET>> GetProjectsAsync();
        public Task<IEnumerable<ProjectGET>> GetProjectsByUserAsync(int userId);
        public Task<int> DeleteProjectByIdAsync(int projectId);
        public Task<ProjectGET> GetProjectByIdAsync(int id);
        Task<int> UpdateProjectAsync(int id, ProjectPATCH project);
    }
}
