using Microsoft.AspNetCore.Mvc;
using Havoc_API.Services;
using Havoc_API.DTOs.Participation;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Havoc_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId}/participations")]
    public class ParticipationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IParticipationService _participationService;

        public ParticipationController(
            IParticipationService participationService,
            IUserService userService)
        {
            _userService = userService;
            _participationService = participationService;
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveParticipation(int userId, int projectId)
        {
            try
            {
                var currentUserId = _userService.GetUserId(Request);
                var role = await _participationService
                    .GetUserRoleInProjectAsync(currentUserId, projectId);
                if (!role.CanEditProject())
                    return Unauthorized("You have no permission to edit project");

                await _participationService.DeleteParticipation(userId, projectId);

                return Ok(new { message = "Participation removed successfully" });
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

        [HttpDelete("leave")]
        public async Task<ActionResult> LeaveProject(int projectId)
        {
            try
            {
                var currentUserId = _userService.GetUserId(Request);
                await _participationService.DeleteParticipation(currentUserId, projectId);

                return Ok(new { messege = "You have successfully left the project" });
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

        [HttpPost]
        public async Task<IActionResult> AddParticipations(
            List<ParticipationPOST> participations, int projectId)
        {
            try
            {
                var currentUserId = _userService.GetUserId(Request);
                var role = await _participationService.GetUserRoleInProjectAsync(currentUserId, projectId);

                if (!role.CanEditProject())
                {
                    return Unauthorized(new { message = "You have no permission to edit project" });
                }

                var result = await _participationService.AddParticipationListAsync(participations);
                return Ok(result);
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

        [HttpPatch]
        public async Task<IActionResult> PatchParticipantRole(int userId, int projectId, ParticipationPATCH patch)
        {
            try
            {
                var currentUserId = _userService.GetUserId(Request);
                var currentRole = await _participationService.GetUserRoleInProjectAsync(currentUserId, projectId);
                if (!currentRole.CanEditProject())
                {
                    return Unauthorized(new { message = "You have no permission to edit project" });
                }

                var updatedParticipation = await _participationService.PatchParticipantRoleAsync(userId, projectId, patch);
                return Ok(updatedParticipation);
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
        public async Task<ActionResult<ICollection<ParticipationGET>>> GetParticipationsByProjectIdAsync(int projectId)
        {
            try
            {
                var participations = await _participationService.GetParticipationsByProjectIDAsync(projectId);
                return Ok(participations);
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
    }
}
