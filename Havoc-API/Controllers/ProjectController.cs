using Havoc_API.Models.DTOs.Project;
using Havoc_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult> getProjects()
        {
            return Ok(await _projectService.getProjectsAsync());
        }
        [HttpPost]
        public async Task<ActionResult> addProject(ProjectPOST newProject)
        {
            try
            {
                return Ok(await _projectService.addProjectAsync(newProject));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
