using Havoc_API.DTOs.Tag;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/projects/{projectId}/tasks/{taskId}/tags")]
[Route("api/tasks/{taskId}/tags")]

[ApiController]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTagsByTaskAsync(int taskId)
    {
        var tags = await _tagService.GetTagsByTaskIdAsync(taskId);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> AddTagToTaskAsync([FromBody] TagPOST tag, [FromRoute] int taskId)
    {
        var newTagId = await _tagService.AddTagToTaskAsync(tag, taskId);
        return Created($"api/tasks/{taskId}/tags/{newTagId}", new { TagId = newTagId });

    }

    [HttpDelete("{tagId}")]
    public async Task<IActionResult> DeleteTagFromTaskAsync([FromRoute] int taskId, [FromRoute] int tagId)
    {
        await _tagService.DeleteTagFromTaskAsync(tagId, taskId);
        return NoContent();
    }
}
