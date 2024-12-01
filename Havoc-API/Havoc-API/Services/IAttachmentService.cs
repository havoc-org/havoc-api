using Havoc_API.DTOs.Attachment;
using Havoc_API.Models;

namespace Havoc_API.Services;

public interface IAttachmentService
{
    public Task<AttachmentGET> AddAttachmentAsync(AttachmentPOST attachment, int taskId, int creatorId);
    public Task<IEnumerable<AttachmentGET>> AddManyAttachmentsAsync(IEnumerable<AttachmentPOST> attachment, int taskId, int creatorId);
    public Task<AttachmentGET> GetAttachmentAsync(int attachmentId);
    public Task<IEnumerable<AttachmentGET>> GetTasksAttachmentsAsync(int taskId);
    public Task<int> DeleteAttachmentAsync(int attachmentId);
}