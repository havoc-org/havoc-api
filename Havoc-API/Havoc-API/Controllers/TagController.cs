using Havoc_API.DTOs.Tag;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Havoc_API.Models;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/projects/{projectId}/tasks/{taskId}/tags")]
[Route("api/tasks/{taskId}/tags")]

[ApiController]
public class TagController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITagService _tagService;
    private readonly IParticipationService _participationService;

    public TagController(ITagService tagService, IUserService userService, IParticipationService participationService)
    {
        _tagService = tagService;
        _userService = userService;
        _participationService = participationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTagsByTaskAsync(int taskId)
    {
        var tags = await _tagService.GetTagsByTaskIdAsync(taskId);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> AddTagsToTaskAsync(IEnumerable<TagPOST> tags, int taskId, int projectId)
    {

        var role =
        await _participationService
            .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);

        if (!role.CanEditTask())
            return Unauthorized(new { message = "You have no permission to edit task" });

        var result = await _tagService.AddTagsToTaskAsync(tags, taskId, projectId);
        return Ok(result);

    }

    [HttpDelete("{tagId}")]
    public async Task<IActionResult> DeleteTagFromTaskAsync( int taskId, int tagId, int projectId)
    {
        var role =
    await _participationService
        .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);

        if (!role.CanEditTask())
            return Unauthorized("You have no permission to edit taôsk");
        var result = await _tagService.DeleteTagFromTaskAsync(tagId, taskId);
        return Ok(new { result });
    }
}
