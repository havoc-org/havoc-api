using Xunit;
using Moq;
using Havoc_API.Controllers;
using Havoc_API.Services;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;
using Havoc_API.DTOs.Comment;
using Havoc_API.Tests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Havoc_API.Tests.ControllersTests;

public class CommentControllerTests
{
    private readonly Mock<ICommentService> _mockCommentService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IParticipationService> _mockParticipationService;
    private readonly CommentController _commentController;

    public CommentControllerTests()
    {
        _mockCommentService = new Mock<ICommentService>();
        _mockUserService = new Mock<IUserService>();
        _mockParticipationService = new Mock<IParticipationService>();
        _commentController = new CommentController
        (
            _mockCommentService.Object,
            _mockUserService.Object,
            _mockParticipationService.Object
        );
    }

    [Fact]
    public async void GetTasksCommentsAsync_ReturnsOkResult_WithListOfTasksComments()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = Havoc_API.Tests.TestData.TaskFactory.Create(user, project);
        var comments = new List<CommentGET>
        {
             CommentFactory.CreateGet
             (
                CommentFactory.Create
                (
                    user, task
                )
            ),
            CommentFactory.CreateGet
             (
                CommentFactory.Create
                (
                    user, task
                )
            ),
            CommentFactory.CreateGet
             (
                CommentFactory.Create
                (
                    user, task
                )
            )
        };
        var taskId = 1;
        _mockCommentService.Setup(service => service.GetTasksCommentsAsync(taskId)).ReturnsAsync(comments);

        // Act
        var result = await _commentController.GetTasksComments(taskId);


        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<CommentGET>>();
        for (int i = 0; i < comments.Count; i++)
            ((OkObjectResult)result).Value.As<IEnumerable<CommentGET>>().ElementAt(i).Should().BeEquivalentTo(comments[i]);

    }

    [Fact]
    public async void AddCommentAsync_ReturnsCreatedAtActionResult_WithNewCommentAndUrlToIt()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        int userId = It.IsAny<int>();
        int taskId = It.IsAny<int>();
        var commentToAdd = CommentFactory.CreatePost();
        commentToAdd.taskId = taskId;
        commentToAdd.projectId = project.ProjectId;
        var commentToGet = CommentFactory.CreateGet
             (
                CommentFactory.Create
                (
                    user, task, "Test comments"
                )
            );
        var commentId = 1;
        commentToGet.CommentId = commentId;

        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockCommentService.Setup(service => service.AddCommentAsync(commentToAdd, userId, taskId)).ReturnsAsync(commentToGet);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());
        // Act
        var result = await _commentController.AddCommentToTask(commentToAdd);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        result.As<CreatedResult>().Location.Should().Be($"api/tasks/{taskId}/comments/{commentId}");
        result.As<CreatedResult>().Value.As<CommentGET>().Should().BeEquivalentTo(commentToGet);
    }

    [Fact]
    public async void DeleteComment_ReturnsNoContentResult_WhenCommentWasDeleted()
    {
        // Arrange
        var user = UserFactory.Create();
        var userId = It.IsAny<int>();
        var project = ProjectFactory.Create(user);
        var projectId = It.IsAny<int>();
        var task = Havoc_API.Tests.TestData.TaskFactory.Create(user, project);
        var taskId = It.IsAny<int>();
        var commentId = It.IsAny<int>();
        var rowsDeleted = It.IsAny<int>();

        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockCommentService.Setup(service => service.DeleteCommentAsync(commentId, projectId)).ReturnsAsync(rowsDeleted);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());

        // Act
        var result = await _commentController.DeleteComment(commentId, projectId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
