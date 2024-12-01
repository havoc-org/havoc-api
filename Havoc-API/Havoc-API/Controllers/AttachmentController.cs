using Havoc_API.DTOs.Attachment;
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
    public AttachmentController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentGET>>> GetAllAttachments(int taskId)
    {
        return Ok(await _attachmentService.GetTasksAttachmentsAsync(taskId));
    }

    [HttpGet("{attachmentId}")]
    public async Task<ActionResult<AttachmentGET>> GetAllAttachments(int taskId, int attachmentId)
    {
        return Ok(await _attachmentService.GetAttachmentAsync(attachmentId));
    }

}