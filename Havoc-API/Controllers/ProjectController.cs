using Havoc_API.DTOs.Project;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                return Ok(await _projectService.GetProjectsAsync());
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetProjectByUserAsync()
        {
            try
            {
                var userId = _userService.GetUserId(Request);

                var projects = await _projectService.GetProjectsByUserAsync(userId);
                return Ok(projects);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddProjectAsync(ProjectPOST newProject)
        {
            try
            {
                var userId = _userService.GetUserId(Request);
                var creator = await _userService.GetUserByIdAsync(userId);
                return Ok(await _projectService.AddProjectAsync(newProject,creator));

            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProjectAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}
