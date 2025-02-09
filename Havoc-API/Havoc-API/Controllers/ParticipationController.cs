using Microsoft.AspNetCore.Mvc;
using Havoc_API.Services;
using Havoc_API.DTOs.Participation;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.AspNetCore.Authorization;

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
            var role = await _participationService
                .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);
            if (!role.CanEditProject())
                return Unauthorized("You have no permission to edit project");

            await _participationService.DeleteParticipation(userId, projectId);

            // Возвращаем JSON с сообщением об успешном удалении
            return Ok(new { message = "Participation removed successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> AddParticipations(
            List<ParticipationPOST> participations, int projectId)
        {
            var role =
                await _participationService
                    .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);

            if (!role.CanEditProject())
            {
                return Unauthorized("You have no permission to edit project");
            }

            var result = await _participationService.AddParticipationListAsync(participations);
            return Ok(result);
        }

        [HttpPatch]
        public async Task<IActionResult> PatchParticipantRole( int userId, int projectId, ParticipationPATCH patch)
        {
            var currentUserId = _userService.GetUserId(Request);
            var currentRole = await _participationService.GetUserRoleInProjectAsync(currentUserId, projectId);
            if (!currentRole.CanEditProject())
            {
                return Unauthorized("You have no permission to edit project");
            }

            try
            {
                var updatedParticipation = await _participationService.PatchParticipantRoleAsync(userId, projectId, patch);
                return Ok(updatedParticipation);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DataAccessException ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
            catch (DataAccessException ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}
