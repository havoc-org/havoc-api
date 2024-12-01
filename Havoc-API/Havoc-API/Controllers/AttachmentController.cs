using Havoc_API.DTOs.Attachment;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers;


[Authorize]
[Route("api/tasks/{taskId}/attachments")]
[ApiController]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IUserService _userService;
    public AttachmentController(IAttachmentService attachmentService, IUserService userService)
    {
        _attachmentService = attachmentService;
        _userService = userService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentGET>>> GetAllAttachments(int taskId)
    {
        var attachments = await _attachmentService.GetTasksAttachmentsAsync(taskId);
        return Ok(attachments);
    }

    [HttpGet("{attachmentId}")]
    public async Task<ActionResult<AttachmentGET>> GetAttachment(int taskId, int attachmentId)
    {
        return Ok(await _attachmentService.GetAttachmentAsync(attachmentId));
    }

    [HttpPost]
    public async Task<ActionResult> AddAttachment(int taskId, AttachmentPOST attachment)
    {
        var newAttachment =
            await _attachmentService.AddAttachmentAsync(attachment, taskId, _userService.GetUserId(Request));
        return Ok(newAttachment);
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveAttachment(int taskId, int attachmentId)
    {
        await _attachmentService.DeleteAttachmentAsync(attachmentId);
        return NoContent();
    }

}