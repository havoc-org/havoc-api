using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.Task;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Xunit;
using Task = Havoc_API.Models.Task;
using AsyncTask = System.Threading.Tasks.Task;
using System.Runtime.CompilerServices;

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
    public async AsyncTask TaskService_GetTasksByProjectIdAsync_ReturnsAllTasksInProject_WhenProjectExisted()
    {
        //Arange
        var user = HavocTestContextFactory.CreateTestUser();
        var project = HavocTestContextFactory.CreateTestProject(user);

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
    public async AsyncTask TaskService_GetTasksByProjectIdAsync_ThrowsNotFoundExeption_WhenProjectDidntExist()
    {
        //Arange
        _context.Projects.RemoveRange(_context.Projects);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.GetTasksByProjectIdAsync(0));
    }

    [Fact]
    public async AsyncTask TaskService_DeleteTaskByIdAsync_ReturnsNumberOfAffectedRows_WhenTaskExisted()
    {
        //Arrange
        var user = HavocTestContextFactory.CreateTestUser();
        var project = HavocTestContextFactory.CreateTestProject(user);

        var task = HavocTestContextFactory.CreateTestTask(user, project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        //Act
        var numberOfLines = await _taskService.DeleteTaskByIdAsync(task.TaskId);

        //Assert
        numberOfLines.Should().BePositive();
        _context.Tasks.Should().NotContainEquivalentOf(task);
    }

    [Fact]
    public async AsyncTask TaskService_DeleteTaskByIdAsync_ThrowsNotFoundException_WhenTaskDidntExist()
    {
        //Arange
        _context.Tasks.RemoveRange(_context.Tasks);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.DeleteTaskByIdAsync(0));
    }

    [Fact]
    public async AsyncTask TaskService_UpdateTaskAsync_ReturnsNumberOfAffectedRows_WhenTaskExisted()
    {
        //Arrange
        var user = HavocTestContextFactory.CreateTestUser();
        var project = HavocTestContextFactory.CreateTestProject(user);

        var task = HavocTestContextFactory.CreateTestTask(user, project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var taskPatch = HavocTestContextFactory.CreateTestTaskPATCH();
        taskPatch.TaskId = task.TaskId;

        //Act
        var numberOfLines = await _taskService.UpdateTaskAsync(taskPatch);

        //Assert
        numberOfLines.Should().BePositive();
        
        var updatedTask = await _context.Tasks.FindAsync(task.TaskId);
        updatedTask.Should().NotBeNull();
        updatedTask!.Name.Should().Be(taskPatch.Name);
        updatedTask.Description.Should().Be(taskPatch.Description);
        updatedTask.Start.Should().Be(taskPatch.Start);
        updatedTask.Deadline.Should().Be(taskPatch.Deadline);
    }

    [Fact]
    public async AsyncTask TaskService_UpdateTaskAsync_ThrowsNotFoundException_WhenTaskDidntExist()
    {
        //Arange
        var taskPatch = HavocTestContextFactory.CreateTestTaskPATCH();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.UpdateTaskAsync(taskPatch));
    }
}
