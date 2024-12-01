using Havoc_API.Data;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services;
public class AttachmentService : IAttachmentService
{
    private readonly IHavocContext _havocContext;
    public AttachmentService(HavocContext havocContext)
    {
        _havocContext = havocContext;
    }

    public async Task<int> AddAttachmentAsync(AttachmentPOST attachment, Models.Task newTask, User creator)
    {
        var newAttachment = new Attachment(attachment.FileLink, newTask, creator);
        await _havocContext.Attachments.AddAsync(newAttachment);
        await _havocContext.SaveChangesAsync();
        return newAttachment.AttachmentId;
    }

    public async Task<IEnumerable<int>> AddManyAttachmentsAsync(IEnumerable<AttachmentPOST> attachments, Models.Task newTask, User creator)
    {
        var newAttachments = attachments.Select(attachment => new Attachment(attachment.FileLink, newTask, creator));
        await _havocContext.Attachments.AddRangeAsync(newAttachments);
        await _havocContext.SaveChangesAsync();
        return newAttachments.Select(s => s.AttachmentId);
    }

    public async Task<int> DeleteAttachmentAsync(int attachmentId)
    {
        var attachment = await _havocContext.Attachments
            .FirstOrDefaultAsync(a => a.AttachmentId == attachmentId)
                ?? throw new NotFoundException("Attachment doesn't exist");
        _havocContext.Attachments.Remove(attachment);
        return await _havocContext.SaveChangesAsync();
    }

    public async Task<AttachmentGET> GetAttachmentAsync(int attachmentId)
    {
        try
        {
            var attachment = await _havocContext.Attachments.FirstOrDefaultAsync(a => a.AttachmentId == attachmentId) ?? throw new NotFoundException("Attachment doesn't exist");
            return new AttachmentGET(
                attachment.AttachmentId,
                attachment.FileLink,
                new UserGET(
                    attachment.UserId,
                    attachment.User.FirstName,
                    attachment.User.LastName,
                    attachment.User.Email
                )
            );
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    public async Task<IEnumerable<AttachmentGET>> GetTasksAttachmentsAsync(int taskId)
    {
        try
        {
            var task = await _havocContext.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId)
                    ?? throw new NotFoundException("Task doesn't exist");

            return task.Attachments
                .Select(attachment => new
                    AttachmentGET(
                        attachment.AttachmentId,
                        attachment.FileLink,
                        new UserGET(
                            attachment.UserId,
                            attachment.User.FirstName,
                            attachment.User.LastName,
                            attachment.User.Email
                        )
                 )).ToList();
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
    }
}