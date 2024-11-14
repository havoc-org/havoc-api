using System;
using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.Assignment;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.Comment;
using Havoc_API.DTOs.Tag;
using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Xunit;
using Task = Havoc_API.Models.Task;
using TaskStatus = Havoc_API.Models.TaskStatus;

namespace Havoc_API.Tests.ServicesTests;

public class TaskServiceTests
{
    private readonly IHavocContext _context;
    private readonly ITaskService _taskService;
    
    public TaskServiceTests()
    {
        _context = HavocTestContextFactory.GetTestContext();

        _taskService = new TaskService(_context);
    }

    [Fact]
    public async System.Threading.Tasks.Task TaskService_GetTasksByProjectIdAsync_ReturnsAllTasksInProject_WhenProjectExisted()
    {
        //Arange
        var user = HavocTestContextFactory.CreateTestUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var project = HavocTestContextFactory.CreateTestProject(user);
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var task1 = HavocTestContextFactory.CreateTestTask(user, project);
        var task2 = HavocTestContextFactory.CreateTestTask(user, project);
        var taskList = new List<Task>([task1, task2]);
        await _context.Tasks.AddRangeAsync(taskList);
        await _context.SaveChangesAsync();

        var taskGet1 = HavocTestContextFactory.CreateTestTaskGET(task1);
        var taskGet2 = HavocTestContextFactory.CreateTestTaskGET(task2);

        //Act
        var tasks = await _taskService.GetTasksByProjectIdAsync(project.ProjectId);

        //Assert
        tasks
            .Should()
            .BeAssignableTo<IEnumerable<TaskGET>>()
            .And.HaveSameCount(taskList)
            .And.ContainEquivalentOf(taskGet1)
            .And.ContainEquivalentOf(taskGet2);
    }

    [Fact]
    public async void TaskService_GetTasksByProjectIdAsync_ThrowsNotFoundExeption_WhenProjectDidntExist()
    {
        //Arange
        _context.Projects.RemoveRange(_context.Projects);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.GetTasksByProjectIdAsync(0));

    }
}
