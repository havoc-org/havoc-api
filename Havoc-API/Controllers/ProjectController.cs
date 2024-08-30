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
        public async Task<ActionResult> getProjects()
        {
            return Ok(await _projectService.getProjectsAsync());
        }


        [HttpGet]
        public async Task<ActionResult> getProjectByUser()
        {
            var userId = _userService.GetUserId(Request);
            
            var projects = await _projectService.getProjectsByUser(userId);
            return Ok(projects);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteProject(int id)
        {
            return Ok();
        }

    }
}
