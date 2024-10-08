using Havoc_API.DTOs.Project;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers
{
    [Authorize]
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public ProjectController(IProjectService projectService, IUserService userService)
        {
            _projectService = projectService;
            _userService = userService;
        }


        [HttpGet("all")]
        public async Task<ActionResult> GetProjectsAsync()
        {
            return Ok(await _projectService.GetProjectsAsync());
        }


        [HttpGet]
        public async Task<ActionResult> GetProjectByUserAsync()
        {
            var userId = _userService.GetUserId(Request);
            
            var projects = await _projectService.GetProjectsByUserAsync(userId);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<ActionResult> AddProjectAsync(ProjectPOST newProject)
        {
            try
            {
                return Ok(await _projectService.AddProjectAsync(newProject));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProjectAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}
