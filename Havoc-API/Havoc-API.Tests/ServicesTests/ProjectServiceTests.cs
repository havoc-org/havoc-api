using System;
using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.Participation;
using Havoc_API.DTOs.Project;
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

    [Fact]
    public async void ProjectService_GetProjectsByUserAsync_ShouldReturnAllProjects_WhereUserParticipates()
    {

        //Arrange
        var user = new User("Test", "Test", "test@test.test", "test");
        var user2 = new User("Test2", "Test2", "test2@test.test", "test2");
        await _context.Users.AddRangeAsync(user, user2);
        await _context.SaveChangesAsync();

        var projectOwnedByUser = new Project
            (
                "Test",
                "test",
                new byte[1234],
                DateTime.Now,
                DateTime.Now.AddDays(34),
                user,
                new ProjectStatus("test")
            );

        var projectWithUsersParticipation = new Project
            (
                "Test",
                "test",
                new byte[1234],
                DateTime.Now,
                DateTime.Now.AddDays(34),
                user2,
                new ProjectStatus("test")
            );

        var projectList = new List<Project>([projectOwnedByUser, projectWithUsersParticipation]);
        await _context.Projects.AddRangeAsync(projectList);
        await _context.SaveChangesAsync();

        var role1 = new Role(RoleType.Developer);
        var role2 = new Role(RoleType.Manager);
        await _context.Roles.AddRangeAsync(role1, role2);
        await _context.SaveChangesAsync();

        var participation = new Participation
            (
                projectWithUsersParticipation,
                role1,
                user
            );

        var participationOwner = new Participation
            (
                projectOwnedByUser,
                role2,
                user
            );

        var participationOwnerForUser2 = new Participation
            (
                projectWithUsersParticipation,
                role2,
                user2
            );
        await _context.Participations.AddRangeAsync(participation, participationOwner, participationOwnerForUser2);
        await _context.SaveChangesAsync();


        var projectOwnedByUserGET = new ProjectGET(
            projectOwnedByUser.ProjectId,
            projectOwnedByUser.Name,
            projectOwnedByUser.Description,
            projectOwnedByUser.Background,
            projectOwnedByUser.Start,
            projectOwnedByUser.Deadline,
            projectOwnedByUser.LastModified,
            new DTOs.User.UserGET(user.UserId, user.FirstName, user.LastName, user.Email),
            new DTOs.ProjectStatus.ProjectStatusGET(projectOwnedByUser.ProjectStatusId, projectOwnedByUser.ProjectStatus.Name),
            [
                new ParticipationGET
                (
                    projectOwnedByUser.ProjectId, new DTOs.User.UserParticipationGET(
                        user.UserId,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        new DTOs.Role.RoleGET(role2.RoleId, role2.Name))
                )
            ]
        );

        var projectWithUsersParticipationGET = new ProjectGET(
            projectWithUsersParticipation.ProjectId,
            projectWithUsersParticipation.Name,
            projectWithUsersParticipation.Description,
            projectWithUsersParticipation.Background,
            projectWithUsersParticipation.Start,
            projectWithUsersParticipation.Deadline,
            projectWithUsersParticipation.LastModified,
            new DTOs.User.UserGET(user2.UserId, user2.FirstName, user2.LastName, user2.Email),
            new DTOs.ProjectStatus.ProjectStatusGET(projectWithUsersParticipation.ProjectStatusId, projectWithUsersParticipation.ProjectStatus.Name),
            [
                new ParticipationGET
                (
                    projectWithUsersParticipation.ProjectId, new DTOs.User.UserParticipationGET(
                        user.UserId,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        new DTOs.Role.RoleGET(role1.RoleId, role1.Name))
                ),
                new ParticipationGET
                (
                    projectWithUsersParticipation.ProjectId, new DTOs.User.UserParticipationGET(
                        user2.UserId,
                        user2.FirstName,
                        user2.LastName,
                        user2.Email,
                        new DTOs.Role.RoleGET(role2.RoleId, role2.Name))
                )
            ]
        );


        //Act
        var projects = await _projectService.GetProjectsByUserAsync(user.UserId);
        //Assert
        projects
            .Should()
            .BeAssignableTo<IEnumerable<ProjectGET>>()
            .And.HaveSameCount(projectList)
            .And.ContainEquivalentOf(projectWithUsersParticipationGET)
            .And.ContainEquivalentOf(projectOwnedByUserGET);
    }

    [Fact]
    public async void ProjectService_GetProjectsAsync_ShouldReturnAllProjects()
    {

        //Arrange
        _context.Projects.RemoveRange(_context.Projects.ToList());
        _context.Participations.RemoveRange(_context.Participations.ToList());

        var user = new User("Test", "Test", "test@test.test", "test");
        var user2 = new User("Test2", "Test2", "test2@test.test", "test2");
        await _context.Users.AddRangeAsync(user, user2);
        await _context.SaveChangesAsync();

        var projectOwnedByUser = new Project
            (
                "Test",
                "test",
                new byte[1234],
                DateTime.Now,
                DateTime.Now.AddDays(34),
                user,
                new ProjectStatus("test")
            );

        var projectWithUsersParticipation = new Project
            (
                "Test",
                "test",
                new byte[1234],
                DateTime.Now,
                DateTime.Now.AddDays(34),
                user2,
                new ProjectStatus("test")
            );
        var projectList = new List<Project>([projectOwnedByUser, projectWithUsersParticipation]);
        await _context.Projects.AddRangeAsync(projectList);
        await _context.SaveChangesAsync();

        var role1 = new Role(RoleType.Developer);
        var role2 = new Role(RoleType.Manager);
        await _context.Roles.AddRangeAsync(role1, role2);
        await _context.SaveChangesAsync();

        var participation = new Participation
            (
                projectWithUsersParticipation,
                role1,
                user
            );

        var participationOwner = new Participation
            (
                projectOwnedByUser,
                role2,
                user
            );

        var participationOwnerForUser2 = new Participation
            (
                projectWithUsersParticipation,
                role2,
                user2
            );
        await _context.Participations.AddRangeAsync(participation, participationOwner, participationOwnerForUser2);
        await _context.SaveChangesAsync();


        var projectOwnedByUserGET = new ProjectGET(
            projectOwnedByUser.ProjectId,
            projectOwnedByUser.Name,
            projectOwnedByUser.Description,
            projectOwnedByUser.Background,
            projectOwnedByUser.Start,
            projectOwnedByUser.Deadline,
            projectOwnedByUser.LastModified,
            new DTOs.User.UserGET(user.UserId, user.FirstName, user.LastName, user.Email),
            new DTOs.ProjectStatus.ProjectStatusGET(projectOwnedByUser.ProjectStatusId, projectOwnedByUser.ProjectStatus.Name),
            [
                new ParticipationGET
                (
                    projectOwnedByUser.ProjectId, new DTOs.User.UserParticipationGET(
                        user.UserId,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        new DTOs.Role.RoleGET(role2.RoleId, role2.Name))
                )
            ]
        );

        var projectWithUsersParticipationGET = new ProjectGET(
            projectWithUsersParticipation.ProjectId,
            projectWithUsersParticipation.Name,
            projectWithUsersParticipation.Description,
            projectWithUsersParticipation.Background,
            projectWithUsersParticipation.Start,
            projectWithUsersParticipation.Deadline,
            projectWithUsersParticipation.LastModified,
            new DTOs.User.UserGET(user2.UserId, user2.FirstName, user2.LastName, user2.Email),
            new DTOs.ProjectStatus.ProjectStatusGET(projectWithUsersParticipation.ProjectStatusId, projectWithUsersParticipation.ProjectStatus.Name),
            [
                new ParticipationGET
                (
                    projectWithUsersParticipation.ProjectId, new DTOs.User.UserParticipationGET(
                        user.UserId,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        new DTOs.Role.RoleGET(role1.RoleId, role1.Name))
                ),
                new ParticipationGET
                (
                    projectWithUsersParticipation.ProjectId, new DTOs.User.UserParticipationGET(
                        user2.UserId,
                        user2.FirstName,
                        user2.LastName,
                        user2.Email,
                        new DTOs.Role.RoleGET(role2.RoleId, role2.Name))
                )
            ]
        );


        //Act
        var projects = await _projectService.GetProjectsByUserAsync(user.UserId);
        //Assert
        projects
            .Should()
            .BeAssignableTo<IEnumerable<ProjectGET>>()
            .And.HaveSameCount(projectList)
            .And.ContainEquivalentOf(projectWithUsersParticipationGET)
            .And.ContainEquivalentOf(projectOwnedByUserGET);
    }

    [Fact]
    public async void ProjectService_AddProjectAsync_ShouldCreateNewProjectStatus_WhenDatabaseDoesntHaveItAlready()
    {
        //Arrange
        var user = new User("Test", "Test", "test@test.test", "test");
        //Act
        //Assert
        user.Should().Be(user);
    }
}
