using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Havoc_API.Services;
using Havoc_API.Data;
using Havoc_API.DTOs.Project;
using Havoc_API.DTOs.ProjectStatus;
using Havoc_API.DTOs.Participation;
using Havoc_API.Models;
using Havoc_API.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;


namespace Havoc_API_TESTS;

public class ProjectServiceTests
{
    private readonly Mock<IHavocContext> _mockContext;
    private readonly Mock<IParticipationService> _mockParticipationService;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        _mockContext = new Mock<IHavocContext>();
        _mockParticipationService = new Mock<IParticipationService>();
        
        _projectService = new ProjectService(_mockContext.Object, _mockParticipationService.Object);
    }


    [Fact]
    public async System.Threading.Tasks.Task AddProjectAsync_Should_Add_Project_When_Date_Is_Valid()
    {
        // Arrange
        var projectPost = new ProjectPOST
        {
            Name = "New Project",
            Description = "Test Project",
            Start = DateTime.Now,
            Deadline = DateTime.Now.AddDays(10),
            CreatorId = 1,
            ProjectStatus = new ProjectStatusPOST { Name = "InProgress" }
        };

        var creator = new User("John", "Doe", "johndoe@gmail.com", "password123");

        _mockContext.Setup(x => x.Users.FindAsync(projectPost.CreatorId)).ReturnsAsync(creator);
        _mockContext.Setup(x => x.ProjectStatuses.AddAsync(It.IsAny<ProjectStatus>())).Returns(System.Threading.Tasks.Task.CompletedTask);
        _mockContext.Setup(x => x.Projects.AddAsync(It.IsAny<Project>())).Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        var result = await _projectService.AddProjectAsync(projectPost);

        // Assert
        _mockContext.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }
}