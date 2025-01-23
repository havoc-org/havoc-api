using Microsoft.AspNetCore.Mvc;
using Havoc_API.Services;
using Havoc_API.DTOs.Assignment;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace Havoc_API.Controllers;


[Authorize]
[ApiController]
[Route("api/projects/{projectId}/tasks/{taskId}/assignments")]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly IUserService _userService;
    private readonly IParticipationService _participationService;

    public AssignmentController(
                    IAssignmentService assignmentService,
                    IUserService userService,
                    IParticipationService participationService
    )
    {
        _assignmentService = assignmentService;
        _userService = userService;
        _participationService = participationService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAssignments(
    IEnumerable<AssignmentPOST> assignments,
    int taskId,
    int projectId)
    {
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);

        if (!role.CanEditTask())
            return Unauthorized("You have no permission to edit task");

        var result = await _assignmentService.AddManyAssignmentsAsync(
            assignments,
            taskId,
            projectId
        );

        return Ok(result);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveAssignment(int taskId, int userId, int projectId)
    {
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);
        if (!role.CanEditTask())
            return Unauthorized("You have no permission to edit task");
        await _assignmentService.DeleteAssignmentAsync(taskId, userId, projectId);
        return NoContent();
    }

}
