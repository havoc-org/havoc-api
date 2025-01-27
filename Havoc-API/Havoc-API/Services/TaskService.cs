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

    public async Task<List<TaskGET>> GetTasksAsync()
    {

        var tasks = await _havocContext.Tasks
        .Select(task => new TaskGET(
            task.TaskId,
            task.Name,
            task.Description,
            task.Start,
            task.Deadline,
            task.ProjectId,
            new UserGET(
                task.Creator.UserId,
                task.Creator.FirstName,
                task.Creator.LastName,
                task.Creator.Email
            ),
            new TaskStatusGET(
                task.TaskStatus.TaskStatusId,
                task.TaskStatus.Name
            ),
            task.Assignments.Select(assignment => new AssignmentGET(
                new UserGET(
                    assignment.UserId,
                    assignment.User.FirstName,
                    assignment.User.LastName,
                    assignment.User.Email
                ),
                assignment.Description
            )).ToList(),
            task.Attachments.Select(attachment => new AttachmentGET(
                attachment.AttachmentId,
                attachment.FileLink,
                new UserGET(
                    attachment.UserId,
                    attachment.User.FirstName,
                    attachment.User.LastName,
                    attachment.User.Email
                )
            )).ToList(),
            task.Comments.Select(comment => new CommentGET(
                comment.CommentId,
                comment.Content,
                comment.CommentDate,
                new UserGET(
                    comment.UserId,
                    comment.User.FirstName,
                    comment.User.LastName,
                    comment.User.Email
                )
            )).ToList(),
            task.Tags.Select(tag => new TagGET(
                tag.TagId,
                tag.Name,
                tag.ColorHex
            )).ToList()
        )).ToListAsync();

        return tasks;
    }

    public async Task<List<TaskGET>> GetTasksByProjectIdAsync(int projectId)
    {
        bool isProjectExist = await _havocContext.Projects.AnyAsync(p => p.ProjectId == projectId);

        if (!isProjectExist)
            throw new NotFoundException("Project Not Found");

        var tasks = await _havocContext.Tasks
        .Where(task => task.ProjectId == projectId)
        .Select(task => new TaskGET(
            task.TaskId,
            task.Name,
            task.Description,
            task.Start,
            task.Deadline,
            task.ProjectId,
            new UserGET(
                task.Creator.UserId,
                task.Creator.FirstName,
                task.Creator.LastName,
                task.Creator.Email
            ),
            new TaskStatusGET(
                task.TaskStatus.TaskStatusId,
                task.TaskStatus.Name
            ),
            task.Assignments.Select(assignment => new AssignmentGET(
                new UserGET(
                    assignment.UserId,
                    assignment.User.FirstName,
                    assignment.User.LastName,
                    assignment.User.Email
                ),
                assignment.Description
            )).ToList(),
            task.Attachments.Select(attachment => new AttachmentGET(
                attachment.AttachmentId,
                attachment.FileLink,
                new UserGET(
                    attachment.UserId,
                    attachment.User.FirstName,
                    attachment.User.LastName,
                    attachment.User.Email
                )
            )).ToList(),
            task.Comments.Select(comment => new CommentGET(
                comment.CommentId,
                comment.Content,
                comment.CommentDate,
                new UserGET(
                    comment.UserId,
                    comment.User.FirstName,
                    comment.User.LastName,
                    comment.User.Email
                )
            )).ToList(),
            task.Tags.Select(tag => new TagGET(
                tag.TagId,
                tag.Name,
                tag.ColorHex
            )).ToList()
        )).ToListAsync();

        return tasks;
    }

    public async Task<int> AddTaskAsync(TaskPOST task)
    {
        using var transaction = _havocContext.Database.BeginTransaction();

        var creator = await _havocContext.Users
        .FindAsync(task.CreatorId) ?? throw new NotFoundException("Creator not found");

        var project = await _havocContext.Projects
        .FindAsync(task.ProjectId) ?? throw new NotFoundException("Project not found");

        var status = await _havocContext.TaskStatuses
        .FirstOrDefaultAsync(ts => ts.Name == task.TaskStatus.Name)
        ?? throw new NotFoundException("Task Status not found");

        var newTask = new Models.Task(
            task.Name,
            task.Description,
            task.Start,
            task.Deadline,
            creator,
            project,
            status
        );

        await _havocContext.Tasks.AddAsync(newTask);
        await _havocContext.SaveChangesAsync();

        foreach (var assignment in task.Assignments)
        {
            var userForAssignment = await _havocContext
            .Users.FindAsync(assignment.UserId) ?? throw new NotFoundException("User for Assignment not found");

            var newAssignment = new Assignment(assignment.Description, newTask, userForAssignment);
            await _havocContext.Assignments.AddAsync(newAssignment);
        }

        await _havocContext.SaveChangesAsync();

        foreach (var attachment in task.Attachments)
        {
            var newAttachment = new Attachment(attachment.FileLink, newTask, creator);
            await _havocContext.Attachments.AddAsync(newAttachment);
        }

        await _havocContext.SaveChangesAsync();

        foreach (var tag in task.Tags)
        {
            var newTag = await _havocContext.Tags
            .FirstOrDefaultAsync(t => t.Name == tag.Name && t.ColorHex == tag.ColorHex);

            if (newTag == null)
            {
                newTag = new Tag(tag.Name, tag.ColorHex);
                await _havocContext.Tags.AddAsync(newTag);
                await _havocContext.SaveChangesAsync();
            }

            newTask.Tags.Add(newTag);
        }

        await _havocContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return newTask.TaskId;
    }

    public async Task<int> DeleteTaskByIdAsync(int taskId)
    {
        var task = await _havocContext.Tasks
        .FindAsync(taskId) ?? throw new NotFoundException("Task not found");

        _havocContext.Tasks.Remove(task);
        return await _havocContext.SaveChangesAsync();
    }

    public async Task<int> UpdateTaskAsync(TaskPATCH taskPatch)
    {
        var task = await _havocContext.Tasks
        .FindAsync(taskPatch.TaskId) ?? throw new NotFoundException("Task not found");

        task.UpdateValue(taskPatch.Name, taskPatch.Description, taskPatch.Start, taskPatch.Deadline);
        _havocContext.Tasks.Update(task);
        return await _havocContext.SaveChangesAsync();
    }

    public async Task<int> UpdateTaskStatusAsync(TaskStatusPATCH taskStatus)
    {
        var task = await _havocContext.Tasks
        .FindAsync(taskStatus.TaskId) ?? throw new NotFoundException("Task not found");

        var status = await _havocContext.TaskStatuses
        .FirstOrDefaultAsync(ts => EF.Functions.Like(ts.Name, taskStatus.Name))
        ?? throw new NotFoundException("Task Status not found");

        task.UpdateStatus(status);
        _havocContext.Tasks.Update(task);
        return await _havocContext.SaveChangesAsync();
    }

    public async Task<TaskGET> GetTaskByIdAsync(int taskId)
    {
        return await _havocContext.Tasks
    .Where(task => task.TaskId == taskId)
    .Select(task => new TaskGET(
        task.TaskId,
        task.Name,
        task.Description,
        task.Start,
        task.Deadline,
        task.ProjectId,
        new UserGET(
            task.Creator.UserId,
            task.Creator.FirstName,
            task.Creator.LastName,
            task.Creator.Email
        ),
        new TaskStatusGET(
            task.TaskStatus.TaskStatusId,
            task.TaskStatus.Name
        ),
        task.Assignments.Select(assignment => new AssignmentGET(
            new UserGET(
                assignment.UserId,
                assignment.User.FirstName,
                assignment.User.LastName,
                assignment.User.Email
            ),
            assignment.Description
        )).ToList(),
        task.Attachments.Select(attachment => new AttachmentGET(
            attachment.AttachmentId,
            attachment.FileLink,
            new UserGET(
                attachment.UserId,
                attachment.User.FirstName,
                attachment.User.LastName,
                attachment.User.Email
            )
        )).ToList(),
        task.Comments.Select(comment => new CommentGET(
            comment.CommentId,
            comment.Content,
            comment.CommentDate,
            new UserGET(
                comment.UserId,
                comment.User.FirstName,
                comment.User.LastName,
                comment.User.Email
            )
        )).ToList(),
        task.Tags.Select(tag => new TagGET(
            tag.TagId,
            tag.Name,
            tag.ColorHex
        )).ToList()
    ))
    .FirstOrDefaultAsync()
    ?? throw new NotFoundException("Task not found");
    }

    public async Task<List<TaskStatusGET>> GetAllTaskStatusesAsync()
    {
        var taskStatuses = await _havocContext.TaskStatuses
        .OrderBy(ts => ts.TaskStatusId)
        .Select(ts => new TaskStatusGET(
            ts.TaskStatusId,
            ts.Name
        )).ToListAsync();

        return taskStatuses;
    }
}