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
                new User("Test", "Test", "test@test.test", "test"),
                new ProjectStatus("test")
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
        _context.Projects.RemoveRange(_context.Projects.Select(p => p).ToList());
        var nonExistentProjectId = 0;
        //Act
        var deletion = async () => await _projectService.DeleteProjectByIdAsync(nonExistentProjectId);

        //Assert
        await deletion.Should().ThrowAsync<NotFoundException>("Project not found");

    }
}
