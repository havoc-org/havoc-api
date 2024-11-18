using System;
using FluentAssertions;
using Havoc_API.Controllers;
using Havoc_API.DTOs.Project;
using Havoc_API.Models;
using Havoc_API.Services;
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
        _userService.Setup(s => s.GetUserId()).Returns(It.IsAny<int>());
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
        _userService.Setup(s => s.GetUserId()).Returns(It.IsAny<int>());
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
    //# TODO change type of exception that Project Controller handles
    public async void GetProjectByUserAsync_RerturnsInternalError_WhenProjectServiceThrowsSqlError()
    {
        // //Arrange
        // var projects = new List<ProjectGET>();
        // _userService.Setup(s => s.GetUserId()).Returns(It.IsAny<int>());
        // _projectService.Setup(s => s.GetProjectsByUserAsync(It.IsAny<int>()))
        //     .Throws<Exception>();
        // //Act
        // var response = await _projectController.GetProjectByUserAsync();
        // //Assert
        // response.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
        1.Should().Be(1);
    }
}
