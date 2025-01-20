using Havoc_API.DTOs.Attachment;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers;


[Authorize]
[Route("api/projects/{projectId}/tasks/{taskId}/attachments")]
[Route("api/tasks/{taskId}/attachments")]
[ApiController]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IUserService _userService;
    private readonly IParticipationService _participationService;
    public AttachmentController(
        IAttachmentService attachmentService,
        IUserService userService,
        IParticipationService participationService)
    {
        _attachmentService = attachmentService;
        _userService = userService;
        _participationService = participationService;

    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentGET>>> GetAllAttachments(int taskId)
    {
        var attachments = await _attachmentService.GetTasksAttachmentsAsync(taskId);
        return Ok(attachments);
    }

    [HttpGet("{attachmentId}")]
    public async Task<ActionResult<AttachmentGET>> GetAttachment(int attachmentId, int taskId, int projectId)
    {
        return Ok(await _attachmentService.GetAttachmentAsync(attachmentId, taskId, projectId));
    }

    [HttpPost]
    public async Task<ActionResult> AddAttachments(IEnumerable<AttachmentPOST> attachments, int taskId, int projectId)
    {
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);
        if (!role.CanEditTask())
            return Unauthorized("You have no permission to edit task");
        var newAttachments =
            await _attachmentService.AddManyAttachmentsAsync(attachments, taskId, _userService.GetUserId(Request), projectId);
        return Ok(newAttachments);
    }

    [HttpDelete("{attachmentId}")]
    public async Task<ActionResult> RemoveAttachment(int attachmentId, int taskId, int projectId)
    {
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(_userService.GetUserId(Request), projectId);
        if (!role.CanEditTask())
            return Unauthorized("You have no permission to edit task");
        await _attachmentService.DeleteAttachmentAsync(attachmentId, taskId, projectId);
        return NoContent();
    }

}