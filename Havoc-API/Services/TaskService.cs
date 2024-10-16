
using Havoc_API.Data;
using Havoc_API.DTOs.Assignment;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.Comment;
using Havoc_API.DTOs.Project;
using Havoc_API.DTOs.Tag;
using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services;

public class TaskService : ITaskService
{
    private readonly IHavocContext _havocContext;

    public TaskService(IHavocContext havocContext)
    {
        _havocContext = havocContext;
    }

    public async Task<List<TaskGET>> GetTasksByProjectIdAsync(int projectId)
    {
        bool isProjectExists = await _havocContext.Projects.AnyAsync(p => p.ProjectId == projectId);
        
        if(!isProjectExists)
            throw new NotFoundException("Project Not Found");
        
        var tasks = await _havocContext.Tasks
        .Where(task => task.ProjectId == projectId)
        .Select(task => new TaskGET(
            task.TaskId,
            task.Name,
            task.Description,
            task.Start,
            task.Deadline,
            new UserGET(
                task.Creator.UserId,
                task.Creator.FirstName,
                task.Creator.LastName,
                task.Creator.Email
            ),
            new ProjectGET(
                task.Project.ProjectId,
                task.Project.Name,
                task.Project.Description,
                task.Project.Background,
                task.Project.Start,
                task.Project.Deadline,
                task.Project.LastModified
            ),
            new TaskStatusGET(
                task.TaskStatus.TaskStatusId,
                task.TaskStatus.Name
            ),
            task.Assignments.Select(assignment => new AssignmentGET(
                assignment.UserId,
                assignment.TaskId,
                assignment.Description
            )).ToList(),
            task.Attachments.Select(attachment => new AttachmentGET(
                attachment.AttachmentId,
                attachment.FileLink
            )).ToList(),
            task.Comments.Select(comment => new CommentGET(
                comment.CommentId,
                comment.Content,
                comment.CommentDate
            )).ToList(),
            task.Tags.Select(tag => new TagGET(
                tag.TagId,
                tag.Name,
                tag.ColorHex
            )).ToList()
        )).ToListAsync();
        
        return tasks;
    }
}