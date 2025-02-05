using Xunit;
using Moq;
using Havoc_API.Controllers;
using Havoc_API.Services;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;
using Havoc_API.DTOs.Assignment;
using Havoc_API.Tests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Havoc_API.Tests.ControllersTests;

public class AssignmentControllerTests
{
    private readonly Mock<IAssignmentService> _mockAssignmentService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IParticipationService> _mockParticipationService;
    private readonly AssignmentController _assignmentController;

    public AssignmentControllerTests()
    {
        _mockAssignmentService = new Mock<IAssignmentService>();
        _mockUserService = new Mock<IUserService>();
        _mockParticipationService = new Mock<IParticipationService>();
        _assignmentController = new AssignmentController
        (
            _mockAssignmentService.Object,
            _mockUserService.Object,
            _mockParticipationService.Object
        );
    }

    [Fact]
    public async void AddAssignment_ReturnsOktActionResult_WithTrue()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        int userId = It.IsAny<int>();
        int taskId = It.IsAny<int>();
        var assignmentToAdd = AssignmentFactory.CreatePost(userId);
        var assignmentToGet = AssignmentFactory.CreateGet
        (
            AssignmentFactory.Create
            (
                user, task
            )
        );
        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockAssignmentService.Setup(service => service.AddAssignmentAsync(assignmentToAdd, taskId)).ReturnsAsync(true);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());

        // Act
        var result = await _assignmentController.AddAssignment(assignmentToAdd, taskId, project.ProjectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.As<bool>().Should().BeTrue();
    }

    [Fact]
    public async void AddAssignmentAsync_ShouldReturnListOfAddedAsssignments_WhenAssignmentsWereSuccessfullyAdded()
    {
        // Arrange
        var user = UserFactory.Create();
        var user1 = UserFactory.Create("task1@gmai.com");
        var user2 = UserFactory.Create("task2@gmai.com");
        var user3 = UserFactory.Create("task3@gmai.com");
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        int userId = It.IsAny<int>();
        int taskId = It.IsAny<int>();
        var assignmentToAdd = new AssignmentPOST[]
        {
            AssignmentFactory.CreatePost(It.IsAny<int>()),
            AssignmentFactory.CreatePost(It.IsAny<int>()),
            AssignmentFactory.CreatePost(It.IsAny<int>()),
            AssignmentFactory.CreatePost(It.IsAny<int>()),
        };
        var assignmentToGet = new AssignmentGET[]
        {
            AssignmentFactory.CreateGet
            (
                AssignmentFactory.Create
                (
                    user, task
                )
            ),
            AssignmentFactory.CreateGet
            (
                AssignmentFactory.Create
                (
                    user1, task
                )
            ),
            AssignmentFactory.CreateGet
            (
                AssignmentFactory.Create
                (
                    user2, task
                )
            ),
            AssignmentFactory.CreateGet
            (
                AssignmentFactory.Create
                (
                    user3, task
                )
            )
        };
        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockAssignmentService.Setup(service => service.AddManyAssignmentsAsync(assignmentToAdd, taskId, project.ProjectId)).ReturnsAsync(assignmentToGet);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());

        // Act
        var result = await _assignmentController.AddAssignments(assignmentToAdd, taskId, project.ProjectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result
            .As<OkObjectResult>()
            .Value
            .As<IEnumerable<AssignmentGET>>()
            .Should()
            .BeEquivalentTo(assignmentToGet);
    }

    [Fact]
    public async void RemoveAssignment_ReturnsNoContentResult_WhenAssignmentWasDeleted()
    {
        // Arrange
        var user = UserFactory.Create();
        var userId = It.IsAny<int>();
        var project = ProjectFactory.Create(user);
        var projectId = It.IsAny<int>();
        var task = TestData.TaskFactory.Create(user, project);
        var taskId = It.IsAny<int>();
        var assignmentId = It.IsAny<int>();
        var rowsDeleted = It.IsAny<int>();

        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockAssignmentService.Setup(service => service.DeleteAssignmentAsync(taskId, userId, projectId)).ReturnsAsync(rowsDeleted);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());

        // Act
        var result = await _assignmentController.RemoveAssignment(taskId, userId, projectId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async void RemoveAssignmentsAsync_ShouldReturnOkWithNumberOfAffectedRows_WhenAssignmentsWereRemovedSuccessfully()
    {
        // Arrange
        var user = UserFactory.Create();
        var user1 = UserFactory.Create("task1@gmai.com");
        var user2 = UserFactory.Create("task2@gmai.com");
        var user3 = UserFactory.Create("task3@gmai.com");
        var userId = It.IsAny<int>();
        var project = ProjectFactory.Create(user);
        var projectId = It.IsAny<int>();
        var task = TestData.TaskFactory.Create(user, project);
        var taskId = It.IsAny<int>();
        var assignmentsToDelete = new AssignmentDELETE[]
        {
            AssignmentFactory.CreateDelete(user.UserId),
            AssignmentFactory.CreateDelete(user1.UserId),
            AssignmentFactory.CreateDelete(user2.UserId),
            AssignmentFactory.CreateDelete(user3.UserId)
        };
        var rowsDeleted = It.IsAny<int>();

        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockAssignmentService.Setup(service => service.DeleteManyAssignmentsAsync(assignmentsToDelete, taskId, projectId)).ReturnsAsync(rowsDeleted);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());

        // Act
        var result = await _assignmentController.RemoveAssignmentsAsync(assignmentsToDelete, taskId, projectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.As<int>().Should().Be(rowsDeleted);
    }
}
