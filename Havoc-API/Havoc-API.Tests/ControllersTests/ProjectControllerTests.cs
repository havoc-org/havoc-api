using System;
using Azure.Core;
using FluentAssertions;
using Havoc_API.Controllers;
using Havoc_API.DTOs.Project;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Havoc_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ControllersTests;

public class ProjectControllerTests
{
    private readonly Mock<IProjectService> _projectService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IParticipationService> _participationService;

    private readonly ProjectController _projectController;
    public ProjectControllerTests()
    {
        //mock services
        _projectService = new Mock<IProjectService>();
        _userService = new Mock<IUserService>();
        _participationService = new Mock<IParticipationService>();
        //SUT
        _projectController = new ProjectController(_projectService.Object, _userService.Object, _participationService.Object);
    }

    [Fact]
    public async void GetProjectByUserAsync_ReturnsOkResponseWithListOfProjects()
    {
        //Arrange
        var projects = new List<ProjectGET>() { It.IsAny<ProjectGET>(), It.IsAny<ProjectGET>(), It.IsAny<ProjectGET>() };
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(It.IsAny<int>());
        _projectService.Setup(s => s.GetProjectsByUserAsync(It.IsAny<int>()))
        .ReturnsAsync(projects);
        //Act
        var response = await _projectController.GetProjectByUserAsync();
        //Assert
        response.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(projects);
    }

    [Fact]
    public async void GetProjectByUserAsync_ReturnsOkResponseWithEmptyList_WhenUserDoesntHaveAnyProjects()
    {
        //Arrange
        var emptyProjectList = new List<ProjectGET>();
        var projects = new List<ProjectGET>() { It.IsAny<ProjectGET>(), It.IsAny<ProjectGET>(), It.IsAny<ProjectGET>() };
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(It.IsAny<int>());
        _projectService.Setup(s => s.GetProjectsByUserAsync(It.IsAny<int>()))
            .ReturnsAsync(emptyProjectList);
        //Act
        var response = await _projectController.GetProjectByUserAsync();
        //Assert
        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(emptyProjectList)
            .And.NotBeEquivalentTo(projects);
    }

    [Fact]
    public async void GetProjectByUserAsync_RerturnsInternalError_WhenProjectServiceThrowsSqlError()
    {
        //Arrange
        var projects = new List<ProjectGET>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(It.IsAny<int>());
        _projectService.Setup(s => s.GetProjectsByUserAsync(It.IsAny<int>()))
            .Throws<DataAccessException>();
        //Act
        var response = await _projectController.GetProjectByUserAsync();
        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async void AddProjectAsync_ReturnsOkResponseWithIdOfCreatedProject_WhenProjectWasSuccessfullyCreated()
    {
        //Arrange
        int userId = It.IsAny<int>();
        var user = It.IsAny<User>();
        var projectPost = It.IsAny<ProjectPOST>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _userService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _projectService.Setup(s => s.AddProjectAsync(projectPost, user)).ReturnsAsync(It.IsAny<int>());
        //Act
        var response = await _projectController.AddProjectAsync(projectPost);
        //Assert
        response.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<int>();
    }

    [Fact]
    public async void AddProjectAsync_ReturnsNotFound_WhenCreatorCannotBeFoundInUserService()
    {
        //Arrange
        int userId = It.IsAny<int>();
        var projectPost = It.IsAny<ProjectPOST>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _userService.Setup(s => s.GetUserByIdAsync(userId)).ThrowsAsync(new NotFoundException());
        _projectService.Setup(s => s.AddProjectAsync(projectPost, It.IsAny<User>()))
            .ReturnsAsync(It.IsAny<int>());
        //Act
        var response = await _projectController.AddProjectAsync(projectPost);
        //Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }
    [Fact]
    public async void AddProjectAsync_ReturnsNotFound_WhenCreatorInProjectServiceIsNull()
    {
        //Arrange
        int userId = It.IsAny<int>();
        var projectPost = It.IsAny<ProjectPOST>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _userService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(It.IsAny<User>);
        _projectService.Setup(s => s.AddProjectAsync(projectPost, It.IsAny<User>()))
            .ThrowsAsync(new NotFoundException());
        //Act
        var response = await _projectController.AddProjectAsync(projectPost);
        //Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void AddProjectAsync_ReturnsNotFound_WhenCreatorInProjectServiceIsNullAndWhenCreatorCannotBeFoundInUserService()
    {
        //Arrange
        int userId = It.IsAny<int>();
        var projectPost = It.IsAny<ProjectPOST>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _userService.Setup(s => s.GetUserByIdAsync(userId)).ThrowsAsync(new NotFoundException());
        _projectService.Setup(s => s.AddProjectAsync(projectPost, It.IsAny<User>()))
            .ThrowsAsync(new NotFoundException());
        //Act
        var response = await _projectController.AddProjectAsync(projectPost);
        //Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void AddProjectAsync_ReturnsInternalError_WhenUserServiceThrowsDataAccessException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        var projectPost = It.IsAny<ProjectPOST>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _userService.Setup(s => s.GetUserByIdAsync(userId)).ThrowsAsync(new DataAccessException());
        _projectService.Setup(s => s.AddProjectAsync(projectPost, It.IsAny<User>()))
            .ReturnsAsync(It.IsAny<int>());
        //Act
        var response = await _projectController.AddProjectAsync(projectPost);
        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async void AddProjectAsync_ReturnsInternalError_WhenProjectServiceThrowsDataAccessException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        var projectPost = It.IsAny<ProjectPOST>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _userService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(It.IsAny<User>());
        _projectService.Setup(s => s.AddProjectAsync(projectPost, It.IsAny<User>()))
            .ThrowsAsync(new DataAccessException());
        //Act
        var response = await _projectController.AddProjectAsync(projectPost);
        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async void AddProjectAsync_ReturnsInternalError_WhenProjectServiceThrowsDataAccessExceptionAndWhenUserServiceThrowsDataAccessException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        var projectPost = It.IsAny<ProjectPOST>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _userService.Setup(s => s.GetUserByIdAsync(userId)).ThrowsAsync(new DataAccessException());
        _projectService.Setup(s => s.AddProjectAsync(projectPost, It.IsAny<User>()))
            .ThrowsAsync(new DataAccessException());
        //Act
        var response = await _projectController.AddProjectAsync(projectPost);
        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(123454)]
    [InlineData(789556)]
    public async void DeleteProjectByIdAsync_ReturnsOkResponseWithNumberOfAffectedRows_WhenProjectWasSuccessfullyDeleted(int linesAffected)
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        var canDeleteRole = new Role(RoleType.Owner);
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ReturnsAsync(canDeleteRole);
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ReturnsAsync(linesAffected);
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be($"Affected rows: {linesAffected}");
    }

    [Theory]
    [InlineData(RoleType.Developer)]
    [InlineData(RoleType.Manager)]
    public async void DeleteProjectByIdAsync_ReturnsUnAuthorizedWithMessage_WhenUserHasntDeletePermitionOfProject(RoleType notOwnerRoleType)
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        var canDeleteRole = new Role(notOwnerRoleType);
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ReturnsAsync(canDeleteRole);
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ReturnsAsync(It.IsAny<int>());
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async void DeleteProjectByIdAsync_ReturnsNotFound_WhenProjectServiceThrowsNotFoundException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        var canDeleteRole = new Role(RoleType.Owner);
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ReturnsAsync(canDeleteRole);
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ThrowsAsync(new NotFoundException());
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void DeleteProjectByIdAsync_ReturnsNotFound_WhenParticipationServiceThrowsNotFoundException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ThrowsAsync(new NotFoundException());
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ReturnsAsync(It.IsAny<int>());
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void DeleteProjectByIdAsync_ReturnsNotFound_WhenParticipationServiceThrowsNotFoundExceptionAndProjectServiceThrowsNotFoundException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ThrowsAsync(new NotFoundException());
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ThrowsAsync(new NotFoundException());
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void DeleteProjectByIdAsync_ReturnsInternalError_WhenProjectServiceThrowsDataAccessException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        var canDeleteRole = new Role(RoleType.Owner);
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ReturnsAsync(canDeleteRole);
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ThrowsAsync(new DataAccessException());
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async void DeleteProjectByIdAsync_ReturnsInternalError_WhenParticipationServiceThrowsDataAccessException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ThrowsAsync(new DataAccessException());
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ReturnsAsync(It.IsAny<int>());
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async void DeleteProjectByIdAsync_ReturnsInternalError_WhenParticipationServiceThrowsDataAccessExceptionAndProjectServiceThrowsDataAccessException()
    {
        //Arrange
        int userId = It.IsAny<int>();
        int projectId = It.IsAny<int>();
        _userService.Setup(s => s.GetUserId(It.IsAny<HttpRequest>())).Returns(userId);
        _participationService.Setup(s => s.GetUserRoleInProjectAsync(userId, projectId))
            .ThrowsAsync(new DataAccessException());
        _projectService.Setup(s => s.DeleteProjectByIdAsync(projectId))
            .ThrowsAsync(new DataAccessException());
        //Act
        var response = await _projectController.DeleteProjectByIdAsync(projectId);
        //Assert
        response.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }
}
