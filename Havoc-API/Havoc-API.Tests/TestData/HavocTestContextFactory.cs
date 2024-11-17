using System;
using Havoc_API.Data;
using Havoc_API.DTOs.Assignment;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.Comment;
using Havoc_API.DTOs.Tag;
using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.DTOs.User;
using Havoc_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Task = Havoc_API.Models.Task;
using TaskStatus = Havoc_API.Models.TaskStatus;

namespace Havoc_API.Tests.TestData;

public static class HavocTestContextFactory
{
    public static HavocContext GetTestContext()
    {
        var options = new DbContextOptionsBuilder<HavocContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        var databaseContext = new HavocContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    public static User CreateTestUser()
    {
        var user = new User("TestName", "TestLastName", "test@test.test", "testPass");

        return user;
    }
    public static Project CreateTestProject(User creator)
    {
        var project = new Project(
            "TestName",
            "Test desciption",
            new byte[1234],
            DateTime.Now,
            DateTime.Now.AddDays(34),
            creator,
            new ProjectStatus("TestProjectStatus")
        );

        return project;
    }

    public static Task CreateTestTask(User creator, Project project)
    {
        var task = new Task(
            "TestName",
            "Test desciption",
            DateTime.Now,
            DateTime.Now.AddDays(34),
            creator,
            project,
            new TaskStatus("TestTaskStatus")
        );
        
        return task;
    }

    public static TaskGET CreateTestTaskGET(Task task)
    {
        var taskGet = new TaskGET(
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

        return taskGet;
    }

    public static TaskStatusPATCH CreateTestTaskStatusPatch()
    {
        var taskStatusPatch = new TaskStatusPATCH()
        {
            Name = "NewTaskStatus"
        };

        return taskStatusPatch;
    }

    public static TaskPATCH CreateTestTaskPATCH()
    {
        var taskPatch = new TaskPATCH
        {
            Name = "TestNewName",
            Description = "Test new description",
            Start = DateTime.Now,
            Deadline = DateTime.Now.AddDays(7)
        };

        return taskPatch;
    }
}