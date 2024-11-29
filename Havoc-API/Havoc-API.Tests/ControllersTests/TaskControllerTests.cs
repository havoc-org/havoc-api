using System.Collections;
using FluentAssertions;
using Havoc_API.Controllers;
using Havoc_API.Data;
using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;
using Task = Havoc_API.Models.Task;
using TaskFactory = Havoc_API.Tests.TestData.TaskFactory;

namespace Havoc_API.Tests.ControllersTests;

public class TaskControllerTests
{
    private readonly IHavocContext _context;
    private readonly Mock<ITaskService> _taskService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IParticipationService> _participationService;

    private readonly TaskController _taskController;

    public TaskControllerTests()
    {
        _context = HavocTestContextFactory.GetTestContext();
        _taskService = new Mock<ITaskService>();
        _userService = new Mock<IUserService>();
        _participationService = new Mock<IParticipationService>();

        _taskController = new TaskController(_taskService.Object, _userService.Object, _participationService.Object);
    }

    [Fact]
    public async AsyncTask TaskController_GetTasksByProjectIdAsync_ReturnsOkResponseWithListOfTask_WhenProjectExistedAndUserWasInThisProject()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var role = RoleFactory.DevRole();
        var participation = ParticipationFactory.Create(project, role, user);
        var task1 = TaskFactory.Create(user, project);
        var task2 = TaskFactory.Create(user, project);

        await _context.Participations.AddAsync(participation);
        await _context.Tasks.AddRangeAsync(task1, task2);
        await _context.SaveChangesAsync();

        var taskGet1 = TaskFactory.CreateGet(task1);
        var taskGet2 = TaskFactory.CreateGet(task2);
        var taskGetList = new List<TaskGET> { taskGet1, taskGet2 };

        var taskStatuses = await _context.TaskStatuses
        .OrderBy(ts => ts.TaskStatusId)
        .Select(ts => new TaskStatusGET(
            ts.TaskStatusId,
            ts.Name
        )).ToListAsync();

        _userService.Setup(us => us.GetUserId(It.IsAny<HttpRequest>()))
        .Returns(user.UserId);
        _participationService.Setup(ps => ps.GetUserRoleInProjectAsync(user.UserId, project.ProjectId))
        .ReturnsAsync(role);
        _taskService.Setup(ts => ts.GetTasksByProjectIdAsync(project.ProjectId))
        .ReturnsAsync(taskGetList);
        _taskService.Setup(ts => ts.GetAllTaskStatusesAsync())
        .ReturnsAsync(taskStatuses);

        //Act
        var response = await _taskController.GetTasksByProjectIdAsync(project.ProjectId);

        //Assert
        response.Should().BeOfType<OkObjectResult>().Which.Value.Should()
        .BeEquivalentTo(new {
            statuses = taskStatuses,
            tasks = taskGetList
            });
    }

    [Fact]
    public async AsyncTask TaskController_GetTasksByProjectIdAsync_ReturnsUnauthorized_WhenUserServiceThrowsUnauthorizedAccessException()
    {
        //Arrange
        var message = "User is not authorized";
        _userService.Setup(us => us.GetUserId(It.IsAny<HttpRequest>())).Throws(new UnauthorizedAccessException(message));

        //Act
        var response = await _taskController.GetTasksByProjectIdAsync(1);

        //Assert
        response.Should().BeOfType<UnauthorizedObjectResult>().Which.Value.Should().BeEquivalentTo(new
        {
            Message = message
        });
    }

    [Fact]
    public async AsyncTask TaskController_GetTasksByProjectIdAsync_ReturnsNotFound_WhenProjectDidntExist()
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var nonExistentProjectId = project.ProjectId;
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        var message = "Project not found";

        _userService.Setup(us => us.GetUserId(It.IsAny<HttpRequest>())).Returns(user.UserId);
        _participationService.Setup(ps => ps.GetUserRoleInProjectAsync(user.UserId, nonExistentProjectId)).ThrowsAsync(new NotFoundException(message));

        //Act
        var response = await _taskController.GetTasksByProjectIdAsync(nonExistentProjectId);

        //Assert
        response.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should().BeEquivalentTo(new
        {
            Message = message
        });
    }

    [Fact]
    public async AsyncTask TaskController_GetTasksByProjectIdAsync_ReturnsNotFound_WhenParticipationInProjectDidntExist()
    {
        //Arrange
        var creator = UserFactory.Create("creator@test.test");
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(creator);
        await _context.Projects.AddAsync(project);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var message = "Participation not found";

        _userService.Setup(us => us.GetUserId(It.IsAny<HttpRequest>())).Returns(user.UserId);
        _participationService.Setup(ps => ps.GetUserRoleInProjectAsync(user.UserId, project.ProjectId)).ThrowsAsync(new NotFoundException(message));

        //Act
        var response = await _taskController.GetTasksByProjectIdAsync(project.ProjectId);

        //Assert
        response.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should().BeEquivalentTo(new
        {
            Message = message
        });
    }

    [Theory]
    [InlineData("UserService")]
    [InlineData("ParticipationService")]
    [InlineData("TaskService")]
    public async AsyncTask TaskController_GetTasksByProjectIdAsync_ReturnsServerError_WhenAnyServiceThrowsDataAccessException(string serviceName)
    {
        //Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        switch (serviceName)
        {
            case "UserService":
                _userService.Setup(us => us.GetUserId(It.IsAny<HttpRequest>()))
                .Throws(new DataAccessException());
                break;
            case "ParticipationService":
                _userService.Setup(us => us.GetUserId(It.IsAny<HttpRequest>()))
                .Returns(user.UserId);
                _participationService.Setup(ps => ps.GetUserRoleInProjectAsync(user.UserId, project.ProjectId))
                .ThrowsAsync(new DataAccessException());
                break;
            case "TaskService":
                _userService.Setup(us => us.GetUserId(It.IsAny<HttpRequest>()))
                .Returns(user.UserId);
                var role = RoleFactory.DevRole();
                _participationService.Setup(ps => ps.GetUserRoleInProjectAsync(user.UserId, project.ProjectId))
                .ReturnsAsync(role);
                _taskService.Setup(ts => ts.GetTasksByProjectIdAsync(project.ProjectId))
                .ThrowsAsync(new DataAccessException());
                break;
        }

        //Act
        var response = await _taskController.GetTasksByProjectIdAsync(project.ProjectId);

        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }
}