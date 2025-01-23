using Microsoft.AspNetCore.Mvc;
using Havoc_API.Services;
using Havoc_API.DTOs.Participation;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace Havoc_API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/participations")]


public class ParticipationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IParticipationService _participationService;

    public ParticipationController(

        IParticipationService participationService,
        IUserService userService


    )
    {
        _userService = userService;
        _participationService = participationService;
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveParticipation(int userId, int projectId){
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);
        if (!role.CanEditProject())
            return Unauthorized("You have no permission to edit project");
        await _participationService.DeleteParticipation(userId, projectId);
        return NoContent();
    }


    [HttpPost]
    public async Task<IActionResult> AddParticipations(
        List<ParticipationPOST> participations, int projectId
    )
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

}