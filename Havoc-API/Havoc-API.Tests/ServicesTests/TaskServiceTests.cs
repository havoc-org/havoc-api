using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.Task;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Xunit;
using Task = Havoc_API.Models.Task;
using AsyncTask = System.Threading.Tasks.Task;
using TaskFactory = Havoc_API.Tests.TestData.TaskFactory;

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
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);

        var task1 = TaskFactory.Create(user, project);
        var task2 = TaskFactory.Create(user, project);
        var taskList = new List<Task> { task1, task2 };
        await _context.Tasks.AddRangeAsync(taskList);
        await _context.SaveChangesAsync();

        var taskGet1 = TaskFactory.CreateGet(task1);
        var taskGet2 = TaskFactory.CreateGet(task2);

        // Act
        var tasks = await _taskService.GetTasksByProjectIdAsync(project.ProjectId);

        // Assert
        tasks
            .Should()
            .BeAssignableTo<IEnumerable<TaskGET>>()
            .And.HaveSameCount(taskList)
            .And.ContainEquivalentOf(taskGet1)
            .And.ContainEquivalentOf(taskGet2);
    }

    [Fact]
    public async AsyncTask TaskService_GetTasksByProjectIdAsync_ThrowsNotFoundException_WhenProjectDidntExist()
    {
        // Arrange
        _context.Projects.RemoveRange(_context.Projects);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.GetTasksByProjectIdAsync(0));
    }

    [Fact]
    public async AsyncTask TaskService_AddTaskAsync_ReturnsIdOfCreatedTask_WhenValidDataProvided()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var status = TaskStatusFactory.Create();

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.TaskStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var statusPost = TaskStatusFactory.CreatePost(status.Name);
        var taskPost = TaskFactory.CreatePost(user.UserId, project.ProjectId, statusPost, user.UserId);

        // Act
        var taskId = await _taskService.AddTaskAsync(taskPost);

        // Assert
        var addedTask = await _context.Tasks.FindAsync(taskId);
        addedTask.Should().NotBeNull();
        addedTask!.Name.Should().Be(taskPost.Name);
        addedTask.Description.Should().Be(taskPost.Description);
        addedTask.Start.Should().Be(taskPost.Start);
        addedTask.Deadline.Should().Be(taskPost.Deadline);
        addedTask.TaskStatus.Name.Should().Be(taskPost.TaskStatus.Name);
        addedTask.Assignments.Should().HaveCount(taskPost.Assignments.Count);
        addedTask.Attachments.Should().HaveCount(taskPost.Attachments.Count);
        addedTask.Tags.Should().HaveCount(taskPost.Tags.Count);
    }

    [Fact]
    public async AsyncTask TaskService_AddTaskAsync_ThrowsNotFoundException_WhenCreatorDidntExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var status = TaskStatusFactory.Create();

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.TaskStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var statusPost = TaskStatusFactory.CreatePost(status.Name);
        var taskPost = TaskFactory.CreatePost(0, project.ProjectId, statusPost, user.UserId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.AddTaskAsync(taskPost));
    }

    [Fact]
    public async AsyncTask TaskService_AddTaskAsync_ThrowsNotFoundException_WhenProjectDidntExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var status = TaskStatusFactory.Create();

        await _context.Users.AddAsync(user);
        await _context.TaskStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var statusPost = TaskStatusFactory.CreatePost(status.Name);
        var taskPost = TaskFactory.CreatePost(user.UserId, 0, statusPost, user.UserId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.AddTaskAsync(taskPost));
    }

    [Fact]
    public async AsyncTask TaskService_AddTaskAsync_ThrowsNotFoundException_WhenTaskStatusDidntExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var status = TaskStatusFactory.Create();

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.TaskStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var reversedName = new string(status.Name.Reverse().ToArray());
        var statusPost = TaskStatusFactory.CreatePost(reversedName);
        var taskPost = TaskFactory.CreatePost(0, project.ProjectId, statusPost, user.UserId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.AddTaskAsync(taskPost));
    }

    [Fact]
    public async AsyncTask TaskService_AddTaskAsync_ThrowsNotFoundException_WhenTaskUserForAssignmentDidntExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var status = TaskStatusFactory.Create();

        await _context.Users.AddAsync(user);
        await _context.Projects.AddAsync(project);
        await _context.TaskStatuses.AddAsync(status);
        await _context.SaveChangesAsync();

        var statusPost = TaskStatusFactory.CreatePost(status.Name);
        var taskPost = TaskFactory.CreatePost(0, project.ProjectId, statusPost, 0);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.AddTaskAsync(taskPost));
    }

    [Fact]
    public async AsyncTask TaskService_DeleteTaskByIdAsync_ReturnsNumberOfAffectedRows_WhenTaskExisted()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);

        var task = TaskFactory.Create(user, project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        // Act
        var numberOfLines = await _taskService.DeleteTaskByIdAsync(task.TaskId);

        // Assert
        numberOfLines.Should().BePositive();
        _context.Tasks.Should().NotContainEquivalentOf(task);
    }

    [Fact]
    public async AsyncTask TaskService_DeleteTaskByIdAsync_ThrowsNotFoundException_WhenTaskDidntExist()
    {
        // Arrange
        _context.Tasks.RemoveRange(_context.Tasks);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.DeleteTaskByIdAsync(0));
    }

    [Fact]
    public async AsyncTask TaskService_UpdateTaskAsync_ReturnsNumberOfAffectedRows_WhenTaskExisted()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);

        var task = TaskFactory.Create(user, project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var taskPatch = TaskFactory.CreatePatch();
        taskPatch.TaskId = task.TaskId;

        // Act
        var numberOfLines = await _taskService.UpdateTaskAsync(taskPatch);

        // Assert
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
        // Arrange
        var taskPatch = TaskFactory.CreatePatch();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.UpdateTaskAsync(taskPatch));
    }

    [Fact]
    public async AsyncTask TaskService_UpdateTaskStatusAsync_ReturnsNumberOfAffectedRows_WhenTaskAndStatusExisted()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);

        var task = TaskFactory.Create(user, project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var taskStatusPatch = TaskStatusFactory.CreatePatch();
        taskStatusPatch.TaskId = task.TaskId;
        var taskStatus = TaskStatusFactory.Create(taskStatusPatch.Name);

        _context.TaskStatuses.Add(taskStatus);
        await _context.SaveChangesAsync();

        // Act
        var numberOfAffectedRows = await _taskService.UpdateTaskStatusAsync(taskStatusPatch);

        // Assert
        numberOfAffectedRows.Should().BePositive();
        var updatedTask = await _context.Tasks.FindAsync(task.TaskId);
        updatedTask.Should().NotBeNull();
        updatedTask!.TaskStatus.Name.Should().Be(taskStatusPatch.Name);
    }

    [Fact]
    public async AsyncTask TaskService_UpdateTaskStatusAsync_ThrowsNotFoundException_WhenTaskDidntExist()
    {
        // Arrange
        var taskStatusPatch = TaskStatusFactory.CreatePatch();
        _context.TaskStatuses.Add(new Models.TaskStatus(taskStatusPatch.Name));
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.UpdateTaskStatusAsync(taskStatusPatch));
    }

    [Fact]
    public async AsyncTask TaskService_UpdateTaskStatusAsync_ThrowsNotFoundException_WhenStatusDidntExist()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);

        var task = TaskFactory.Create(user, project);
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var taskStatusPatch = TaskStatusFactory.CreatePatch();
        taskStatusPatch.TaskId = task.TaskId;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.UpdateTaskStatusAsync(taskStatusPatch));
    }
}
