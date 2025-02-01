using Havoc_API.DTOs.Assignment;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.Comment;
using Havoc_API.DTOs.Tag;
using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.DTOs.User;
using Havoc_API.Models;
using Task = Havoc_API.Models.Task;
using TaskStatus = Havoc_API.Models.TaskStatus;

namespace Havoc_API.Tests.TestData;

public static class TaskFactory
{
    public static Task Create(User creator, Project project)
    {
        return new Task(
            "TestName",
            "Test description",
            DateTime.Now,
            DateTime.Now.AddDays(34),
            creator,
            project,
            new TaskStatus("TestTaskStatus")
        );
    }

    public static TaskGET CreateGet(Task task)
    {
        return new TaskGET
        (
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
        );
    }

    public static TaskPOST CreatePost(int creatorId, int projectId, TaskStatusPOST taskStatus, int userForAssignmentId)
    {
        return new TaskPOST
        {
            Name = "Test Task Name",
            Description = "Test task description",
            Start = DateTime.Now,
            Deadline = DateTime.Now.AddDays(7),
            TaskStatus = taskStatus,
            CreatorId = creatorId,
            ProjectId = projectId,
            Assignments = new List<AssignmentPOST>
            {
                new AssignmentPOST
                {
                    UserId = userForAssignmentId,
                    Description = "Test assignment description"
                }
            },
            Attachments = new List<AttachmentPOST>
            {
                new AttachmentPOST
                {
                    FileLink = "http://example.com/testfile"
                }
            },
            Tags = new List<TagPOST>
            {
                new TagPOST
                {
                    Name = "Test Tag",
                    ColorHex = "#FF5733"
                }
            }
        };
    }

    public static TaskPATCH CreatePatch()
    {
        return new TaskPATCH
        {
            Name = "TestNewName",
            Description = "Test new description",
            Start = DateTime.Now,
            Deadline = DateTime.Now.AddDays(7)
        };
    }
}