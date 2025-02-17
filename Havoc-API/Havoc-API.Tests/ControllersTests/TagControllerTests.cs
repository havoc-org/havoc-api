using Havoc_API.Controllers;
using Havoc_API.Services;
using Moq;
using Xunit;
using Havoc_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Havoc_API.DTOs.Tag;
using FluentAssertions;
using Havoc_API.DTOs.Task;
using Havoc_API.Tests.TestData;

namespace Havoc_API.Tests.ControllersTests;
public class TagControllerTests
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IParticipationService> _participationService;
    private readonly Mock<ITagService> _tagService;
    private readonly TagController _tagController;
    public TagControllerTests()
    {
        _participationService = new Mock<IParticipationService>();
        _tagService = new Mock<ITagService>();
        _userService = new Mock<IUserService>();
        _tagController = new TagController(_tagService.Object, _userService.Object, _participationService.Object);
    }

    [Fact]
    public async void GetTagsByTaskAsync_ReturnsOkResult_WithListOfTags()
    {
        // Arrange
        var taskId = It.IsAny<int>();
        var userId = It.IsAny<int>();
        var projectId = It.IsAny<int>();

        var tags = new List<TagGET>
        {
            TagFactory.CreateGet(TagFactory.Create()),
            TagFactory.CreateGet(TagFactory.Create(colorHex: "#FFFAAA")),
            TagFactory.CreateGet(TagFactory.Create(colorHex: "#888AAA")),
            TagFactory.CreateGet(TagFactory.Create(colorHex: "#3467AA")),
        };
        _participationService.Setup(service => service.GetUserRoleInProjectAsync(userId, projectId)).ReturnsAsync(RoleFactory.OwnerRole());
        _tagService.Setup(service => service.GetTagsByTaskIdAsync(taskId)).ReturnsAsync(tags);

        // Act
        var result = await _tagController.GetTagsByTaskAsync(taskId);

        // Assert
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which
            .Value
            .As<IEnumerable<TagGET>>()
            .Should()
            .BeEquivalentTo(tags);
    }

    [Fact]
    public async void AddTagsToTaskAsync_ReturnsOkWithAddedTags_WhenTagsWereAddedSuccessfully()
    {
        // Arrange
        var userId = It.IsAny<int>();
        var projectId = It.IsAny<int>();
        var tagsToAdd = new TagPOST[]
        {
            TagFactory.CreatePost(colorHex: "#FFFAAA"),
            TagFactory.CreatePost(colorHex: "#FF45AA"),
            TagFactory.CreatePost(colorHex: "#FAA45A"),
            TagFactory.CreatePost(colorHex: "#445FCA"),
        };
        var tags = new List<TagGET>
        {
            TagFactory.CreateGet(TagFactory.Create(colorHex: "#FFFAAA")),
            TagFactory.CreateGet(TagFactory.Create(colorHex: "#FF45AA")),
            TagFactory.CreateGet(TagFactory.Create(colorHex: "#FAA45A")),
            TagFactory.CreateGet(TagFactory.Create(colorHex: "#445FCA")),
        };
        _tagService.Setup(service => service.AddTagsToTaskAsync(tagsToAdd, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(tags);
        _participationService.Setup(service => service.GetUserRoleInProjectAsync(userId, projectId)).ReturnsAsync(RoleFactory.OwnerRole());
        // Act
        var result = await _tagController.AddTagsToTaskAsync(tagsToAdd, It.IsAny<int>(), It.IsAny<int>());

        // Assert
        result
            .Should()
            .BeOfType<OkObjectResult>()
            .Which
            .Value
            .As<IEnumerable<TagGET>>()
            .Should()
            .BeEquivalentTo(tags);
    }

    [Fact]
    public async void DeleteTagFromTaskAsync_ReturnsNoContent_WhenTagIsSuccessfullyDeleted()
    {
        // Arrange
        int taskId = It.IsAny<int>();
        int tagId = It.IsAny<int>();
        var userId = It.IsAny<int>();
        var projectId = It.IsAny<int>();
        int rowsAffected = It.IsAny<int>();
        _tagService.Setup(service => service.DeleteTagFromTaskAsync(tagId, tagId)).ReturnsAsync(rowsAffected);
        _participationService.Setup(service => service.GetUserRoleInProjectAsync(userId, projectId)).ReturnsAsync(RoleFactory.OwnerRole());
        // Act
        var result = await _tagController.DeleteTagFromTaskAsync(taskId, tagId, projectId);
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}