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

    public async Task<AttachmentGET> AddAttachmentAsync(AttachmentPOST attachment, int taskId, int creatorId, int projectId)
    {
        try
        {
            if (await _havocContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.ProjectId == projectId) is null)
                throw new NotFoundException("Task or Project doesnt exist");
            var creator = await _havocContext.Users.FirstOrDefaultAsync(u => u.UserId == creatorId)
                ?? throw new NotFoundException("User doesn't exist");
            var task = await _havocContext.Tasks.FirstOrDefaultAsync(u => u.TaskId == taskId)
                ?? throw new NotFoundException("Task doesn't exist");
            var oldAttachment = await _havocContext.Attachments
                .Include(a => a.User)
                .FirstOrDefaultAsync(
                        a => a.FileLink == attachment.FileLink &&
                        a.TaskId == taskId
                );
            if (oldAttachment is not null)
                return new
                    AttachmentGET(
                        oldAttachment.AttachmentId,
                        oldAttachment.FileLink,
                        new UserGET(
                            oldAttachment.UserId,
                            oldAttachment.User.FirstName,
                            oldAttachment.User.LastName,
                            oldAttachment.User.Email
                        )
                    );
            var newAttachment = new Attachment(attachment.FileLink, task, creator);
            await _havocContext.Attachments.AddAsync(newAttachment);
            await _havocContext.SaveChangesAsync();
            return new
                    AttachmentGET(
                        newAttachment.AttachmentId,
                        newAttachment.FileLink,
                        new UserGET(
                            newAttachment.UserId,
                            newAttachment.User.FirstName,
                            newAttachment.User.LastName,
                            newAttachment.User.Email
                        )
                    );
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    public async Task<IEnumerable<AttachmentGET>> AddManyAttachmentsAsync(IEnumerable<AttachmentPOST> attachments, int taskId, int creatorId, int projectId)
    {
        try
        {
            if (await _havocContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.ProjectId == projectId) is null)
                throw new NotFoundException("Task or Project doesnt exist");
            var creator = await _havocContext.Users.FirstOrDefaultAsync(u => u.UserId == creatorId)
                ?? throw new NotFoundException("User doesn't exist");
            var task = await _havocContext.Tasks.FirstOrDefaultAsync(u => u.TaskId == taskId)
                ?? throw new NotFoundException("Task doesn't exist");
            var newAttachments = attachments.Select(attachment => new Attachment(attachment.FileLink, task, creator));
            await _havocContext.Attachments.AddRangeAsync(newAttachments);
            await _havocContext.SaveChangesAsync();
            return newAttachments.Select(attachment => new
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
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    public async Task<int> DeleteAttachmentAsync(int attachmentId, int taskId, int projectId)
    {
        try
        {
            var attachment = await _havocContext.Attachments
                .Include(a => a.Task)
                .FirstOrDefaultAsync(a =>
                                        a.AttachmentId == attachmentId &&
                                        a.Task.ProjectId == projectId &&
                                        a.TaskId == taskId)
                    ?? throw new NotFoundException("Attachment doesn't exist");
            _havocContext.Attachments.Remove(attachment);
            return await _havocContext.SaveChangesAsync();
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    public async Task<AttachmentGET> GetAttachmentAsync(int attachmentId, int taskId, int projectId)
    {
        try
        {
            var attachment = await _havocContext.Attachments
                .Include(a => a.User)
                .Include(a => a.Task)
                .FirstOrDefaultAsync(a =>
                                        a.AttachmentId == attachmentId &&
                                        a.Task.ProjectId == projectId &&
                                        a.TaskId == taskId)
                    ?? throw new NotFoundException("Attachment doesn't exist");
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
            if (await _havocContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId) is null)
                throw new NotFoundException("Task doesnt exist");
            var attachments = await _havocContext.Attachments
                .Include(a => a.User)
                .Where(a => a.TaskId == taskId)
                .ToListAsync();
            return attachments
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