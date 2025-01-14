using Havoc_API.DTOs.Attachment;
using Havoc_API.Models;

namespace Havoc_API.Services;

public interface IAttachmentService
{
    public Task<AttachmentGET> AddAttachmentAsync(AttachmentPOST attachment, int taskId, int creatorId, int projectId);
    public Task<IEnumerable<AttachmentGET>> AddManyAttachmentsAsync(IEnumerable<AttachmentPOST> attachment, int taskId, int creatorId, int projectId);
    public Task<AttachmentGET> GetAttachmentAsync(int attachmentId, int taskId, int projectId);
    public Task<IEnumerable<AttachmentGET>> GetTasksAttachmentsAsync(int taskId);
    public Task<int> DeleteAttachmentAsync(int attachmentId, int taskId, int projectId);
}