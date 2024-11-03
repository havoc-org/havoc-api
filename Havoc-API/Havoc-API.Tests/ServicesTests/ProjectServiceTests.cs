using System;
using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ServicesTests;

public class ProjectServiceTests
{
    private readonly Mock<IParticipationService> _participationService;
    private readonly IProjectService _projectService;
    private readonly IHavocContext _context;
    public ProjectServiceTests()
    {
        //mock services
        _participationService = new Mock<IParticipationService>();
        _context = HavocTestContextFactory.GetTestContext();
        //SUT
        _projectService = new ProjectService(_context, _participationService.Object);

    }

    [Fact]
    public async void ProjectService_DeleteProjectByIdAsync_ReturnsNumberOfAffectedRows_WhenProjectExisted()
    {
        //Arrange
        var project = new Project
            (
                "Test",
                "test",
                new byte[1234],
                DateTime.Now,
                DateTime.Now.AddDays(34),
                new Mock<User>().Object,
                new Mock<ProjectStatus>().Object
            );
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        //Act
        var numberOfLines = await _projectService.DeleteProjectByIdAsync(project.ProjectId);
        //Assert
        numberOfLines.Should().BePositive();
        _context.Projects.Should().NotContainEquivalentOf(project);
    }

    [Fact]
    public async void ProjectService_DeleteProjectByIdAsync_ThrowsNotFoundException_WhenProjectDidntExist()
    {
        //Arrange
        var project = new Project
            (
                "Test",
                "test",
                new byte[1234],
                DateTime.Now,
                DateTime.Now.AddDays(34),
                new Mock<User>().Object,
                new Mock<ProjectStatus>().Object
            );
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        //Act
        var deletion = async () => await _projectService.DeleteProjectByIdAsync(project.ProjectId);

        //Assert
        await deletion.Should().ThrowAsync<NotFoundException>("Project not found");

    }
}
