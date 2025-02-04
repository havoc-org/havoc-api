using Xunit;
using Moq;
using Havoc_API.Controllers;
using Havoc_API.Services;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;
using Havoc_API.DTOs.Attachment;
using Havoc_API.Tests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Havoc_API.Tests.ControllersTests;

public class AttachmentControllerTests
{
    private readonly Mock<IAttachmentService> _mockAttachmentService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IParticipationService> _mockParticipationService;
    private readonly AttachmentController _attachmentController;

    public AttachmentControllerTests()
    {
        _mockAttachmentService = new Mock<IAttachmentService>();
        _mockUserService = new Mock<IUserService>();
        _mockParticipationService = new Mock<IParticipationService>();
        _attachmentController = new AttachmentController
        (
            _mockAttachmentService.Object,
            _mockUserService.Object,
            _mockParticipationService.Object
        );
    }

    [Fact]
    public async void GetAllAttachments_ReturnsOkResult_WithListOfTasksAttachments()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        var attachments = new List<AttachmentGET>
        {
            AttachmentFactory.CreateGet
            (
                AttachmentFactory.Create
                (
                    user, task
                )
            ),
            AttachmentFactory.CreateGet
            (
                AttachmentFactory.Create
                (
                    user, task
                )
            ),
            AttachmentFactory.CreateGet
            (
                AttachmentFactory.Create
                (
                    user, task
                )
            )
        };
        var taskId = task.TaskId;
        _mockAttachmentService.Setup(service => service.GetTasksAttachmentsAsync(taskId)).ReturnsAsync(attachments);

        // Act
        var result = await _attachmentController.GetAllAttachments(taskId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        result.Result.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<AttachmentGET>>();
        for (int i = 0; i < attachments.Count; i++)
            result.Result.As<OkObjectResult>()
            .Value.As<IEnumerable<AttachmentGET>>()
            .ElementAt(i).Should().BeEquivalentTo(attachments[i]);
    }

    [Fact]
    public async void AddAttachments_ReturnsOkActionResult_WithListOfNewAttachments()
    {
        // Arrange
        var user = UserFactory.Create();
        var project = ProjectFactory.Create(user);
        var task = TestData.TaskFactory.Create(user, project);
        int userId = It.IsAny<int>();
        int taskId = It.IsAny<int>();
        var attachmentToAdd = new List<AttachmentPOST>()
        {
            AttachmentFactory.CreatePost(),
            AttachmentFactory.CreatePost(fileLink: "Link2"),
            AttachmentFactory.CreatePost(fileLink: "Link3")
        };
        var attachmentToGet = new List<AttachmentGET>()
        {
            AttachmentFactory.CreateGet
            (
                AttachmentFactory.Create
                (
                    user, task, attachmentToAdd[0].FileLink
                )
            ),

            AttachmentFactory.CreateGet
            (
                AttachmentFactory.Create
                (
                    user, task, attachmentToAdd[1].FileLink
                )
            ),

            AttachmentFactory.CreateGet
            (
                AttachmentFactory.Create
                (
                    user, task, attachmentToAdd[0].FileLink
                )
            )
        };

        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockAttachmentService.Setup(service => service.AddManyAttachmentsAsync(attachmentToAdd, taskId, userId, project.ProjectId)).ReturnsAsync(attachmentToGet);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());

        // Act
        var result = await _attachmentController.AddAttachments(attachmentToAdd, taskId, project.ProjectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.As<IEnumerable<AttachmentGET>>().Should().BeEquivalentTo(attachmentToGet);
    }

    [Fact]
    public async void RemoveAttachment_ReturnsNoContentResult_WhenAttachmentWasUnattchedFromTask()
    {
        // Arrange
        var user = UserFactory.Create();
        var userId = It.IsAny<int>();
        var project = ProjectFactory.Create(user);
        var projectId = It.IsAny<int>();
        var task = Havoc_API.Tests.TestData.TaskFactory.Create(user, project);
        var taskId = It.IsAny<int>();
        var attachmentId = It.IsAny<int>();
        var rowsDeleted = It.IsAny<int>();

        _mockUserService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _mockAttachmentService.Setup(service => service.DeleteAttachmentAsync(attachmentId, taskId, projectId)).ReturnsAsync(rowsDeleted);
        _mockParticipationService.Setup(service => service.GetUserRoleInProjectAsync(userId, project.ProjectId)).ReturnsAsync(RoleFactory.OwnerRole());

        // Act
        var result = await _attachmentController.RemoveAttachment(attachmentId, taskId, projectId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
