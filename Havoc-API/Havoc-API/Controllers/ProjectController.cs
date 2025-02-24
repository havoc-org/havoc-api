﻿using Havoc_API.DTOs.Project;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IParticipationService _participationService;

        public ProjectController(
            IProjectService projectService,
            IUserService userService,
            IParticipationService participationService)
        {
            _projectService = projectService;
            _userService = userService;
            _participationService = participationService;
        }

        [HttpPatch("{projectId}")]
        public async Task<ActionResult> UpdateProjectAsync(int projectId, [FromBody] ProjectPATCH projectUpdate)
        {
            try
            {
                var userId = _userService.GetUserId(Request);

                var role = await _participationService.GetUserRoleInProjectAsync(userId, projectId);
                if (!role.CanEditProject())
                    return Unauthorized(new { message = "You have no permission to edit this project" });

                var result = await _projectService.UpdateProjectAsync(projectId, projectUpdate);

                return Ok(new { AffectedRows = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (DataAccessException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProjectByIdAsync(int id)
        {
            try
            {
                var projects = await _projectService.GetProjectByIdAsync(id);
                return Ok(projects);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ex.Message });
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
            catch (DataAccessException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddProjectAsync(ProjectPOST newProject)
        {
            try
            {
                var userId = _userService.GetUserId(Request);
                var creator = await _userService.GetUserByIdAsync(userId);
                return Ok(await _projectService.AddProjectAsync(newProject, creator));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (DataAccessException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpDelete("{projectId}")]
        public async Task<ActionResult> DeleteProjectByIdAsync(int projectId)
        {
            try
            {
                var creatorId = _userService.GetUserId(Request);
                var role = await _participationService.GetUserRoleInProjectAsync(creatorId, projectId);
                if (!role.CanDeleteProject())
                    return Unauthorized(new { message = "You have no permission to delete this project" });

                var result = await _projectService.DeleteProjectByIdAsync(projectId);
                return Ok(new { AffectedRows = result, Message = "Project deleted successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (DataAccessException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPost("{inviteCode}")]
        public async Task<ActionResult> AddUserToProjectThroughInviteCodeAsync(string inviteCode)
        {
            try
            {
                var userId = _userService.GetUserId(Request);
                var result = await _participationService.AddUserToProjectThroughInviteCodeAsync(userId, inviteCode);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}
