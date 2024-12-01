using Havoc_API.DTOs.Attachment;
using Havoc_API.Models;

namespace Havoc_API.Services;

public interface IAttachmentService
{
    public Task<int> AddAttachmentAsync(AttachmentPOST attachment, Models.Task newTask, User creator);
    public Task<IEnumerable<int>> AddManyAttachmentsAsync(IEnumerable<AttachmentPOST> attachment, Models.Task newTask, User creator);
    public Task<AttachmentGET> GetAttachmentAsync(int attachmentId);
    public Task<IEnumerable<AttachmentGET>> GetTasksAttachmentsAsync(int taskId);
    public Task<int> DeleteAttachmentAsync(int attachmentId);
}