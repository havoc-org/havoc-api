using System;
using Havoc_API.Controllers;
using Havoc_API.Services;
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
    public void GetProjectByUserAsync_ReturnsOkResponseWithListOfProjects()
    {
        //Arrange
        //Act
        //Assert
    }

    [Fact]
    public void GetProjectByUserAsync_ReturnsOkResponseWithEmptyList_WhenUserDoesntHaveAnyProjects()
    {
        //Arrange
        //Act
        //Assert
    }

    [Fact]
    public void GetProjectByUserAsync_RerturnsInternalError_ProjectServiceThrowsSqlError()
    {
        //Arrange
        //Act
        //Assert
    }
}
