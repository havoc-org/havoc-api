using System;
using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.Participation;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ServicesTests;

public class ParticipationServiceTests
{
    private readonly IHavocContext _context;
    private readonly IParticipationService _participationService;
    public ParticipationServiceTests()
    {
        _context = HavocTestContextFactory.GetTestContext();

        //SUT
        _participationService = new ParticipationService(_context);
    }

    [Fact]
    public async void ParticipationService_GetUserRoleInProjectAsync_ShouldReturnUsersRole()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );

        var participationNotOwner = new Participation
            (
                project,
                role2,
                user2
            );
        await _context.Participations.AddRangeAsync(participationNotOwner, participationOwner);
        await _context.SaveChangesAsync();
        //Act
        var ownerRole = await _participationService.GetUserRoleInProjectAsync(user1.UserId, project.ProjectId);
        var participantRole = await _participationService.GetUserRoleInProjectAsync(user2.UserId, project.ProjectId);
        //Assert
        ownerRole.Should().BeEquivalentTo(role1);
        participantRole.Should().BeEquivalentTo(role2);
    }

    [Fact]
    public async void ParticipationService_GetUserRoleInProjectAsync_ShouldThrowNotFoundException_WhenParticipationDidntExist()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );

        var participationNotOwner = new Participation
            (
                project,
                role2,
                user2
            );
        await _context.Participations.AddRangeAsync(participationNotOwner, participationOwner);
        await _context.SaveChangesAsync();
        //Act
        var ownerRoleGetting =
            async () => await _participationService.GetUserRoleInProjectAsync(It.Is<int>(i => i != user1.UserId && i != user2.UserId), project.ProjectId);
        var participantRoleGetting =
            async () => await _participationService.GetUserRoleInProjectAsync(It.Is<int>(i => i != user1.UserId && i != user2.UserId), project.ProjectId);
        //Assert
        await ownerRoleGetting.Should().ThrowAsync<NotFoundException>("Participation not found");
        await participantRoleGetting.Should().ThrowAsync<NotFoundException>("Participation not found");
    }
    [Fact]
    public async void ParticipationService_AddParticipationAsync_ShouldThrowNotFoundException_WhenRoleCannotBeFound()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );

        await _context.Participations.AddAsync(participationOwner);
        await _context.SaveChangesAsync();

        var participationsCount = _context.Participations.Count();
        var roleNotInDb = RoleFactory.ManagerRole();
        var participationPost = new ParticipationPOST(project.ProjectId, user2.Email, roleNotInDb.Name);

        //Act
        var action = async () => await _participationService.AddParticipationAsync(participationPost);

        //Asert
        await action.Should().ThrowAsync<NotFoundException>("Cannot find Role: " + roleNotInDb.Name);
        _context.Participations.Should().HaveCount(participationsCount);
        _context.Participations.Should().ContainEquivalentOf(participationOwner);
    }

    [Fact]
    public async void ParticipationService_AddParticipationAsync_ShouldThrowException_WhenUserNotFound()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );

        await _context.Participations.AddAsync(participationOwner);
        await _context.SaveChangesAsync();

        var participationsCount = _context.Participations.Count();
        var participationPost = new ParticipationPOST
        (
            project.ProjectId,
            It.Is<string>(email => email != "test@test.test" && email != "test2@test.test"),
            role2.Name
        );

        //Act
        var action = async () => await _participationService.AddParticipationAsync(participationPost);

        //Asert

        await action.Should().ThrowAsync<Exception>("User not found");
        _context.Participations.Should().HaveCount(participationsCount);
        _context.Participations.Should().ContainEquivalentOf(participationOwner);
    }

    [Fact]
    public async void ParticipationService_AddParticipationAsync_ShouldThrowException_WhenProjectNotFound()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );

        await _context.Participations.AddAsync(participationOwner);
        await _context.SaveChangesAsync();

        var participationsCount = _context.Participations.Count();
        var participationPost = new ParticipationPOST
        (
            It.Is<int>(projectId => projectId != project.ProjectId),
            user2.Email,
            role2.Name
        );

        //Act
        var action = async () => await _participationService.AddParticipationAsync(participationPost);

        //Asert
        await action.Should().ThrowAsync<Exception>("Project not found");
        _context.Participations.Should().HaveCount(participationsCount);
        _context.Participations.Should().ContainEquivalentOf(participationOwner);
    }

    [Fact]
    public async void ParticipationService_AddParticipationAsync_ShouldThrowException_WhenParticipationtAlreadyExists()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );
        var existingParticipation = new Participation
            (
                project,
                role2,
                user2
            );

        await _context.Participations.AddRangeAsync(participationOwner, existingParticipation);
        await _context.SaveChangesAsync();

        var participationsCount = _context.Participations.Count();
        var participationPost = new ParticipationPOST
        (
            It.Is<int>(projectId => projectId != project.ProjectId),
            user2.Email,
            role2.Name
        );

        //Act
        var action = async () => await _participationService.AddParticipationAsync(participationPost);

        //Asert
        await action.Should().ThrowAsync<Exception>("This participation already exists userID: " + existingParticipation.UserId + " projectID: " + existingParticipation.ProjectId);
        _context.Participations.Should().HaveCount(participationsCount);
        _context.Participations.Should().ContainEquivalentOf(participationOwner);
    }

    [Fact]
    public async void ParticipationService_AddParticipationAsync_ShouldThrowException_WhenParticipationtSuccessfullyCreated()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );

        await _context.Participations.AddRangeAsync(participationOwner);
        await _context.SaveChangesAsync();

        var participationsCount = _context.Participations.Count();
        var participationPost = new ParticipationPOST
        (
            project.ProjectId,
            user2.Email,
            role2.Name
        );

        //Act
        var addParticipation = await _participationService.AddParticipationAsync(participationPost);
        var newParticipation = await _context.Participations.Where(p => p.UserId != user1.UserId).FirstOrDefaultAsync();
        //Asert
        addParticipation.Should().BeTrue();
        _context.Participations.Should().HaveCount(participationsCount + 1);
        _context.Participations.Should().ContainEquivalentOf(participationOwner).And.ContainEquivalentOf(newParticipation);
    }

    [Fact]
    public async void ParticipationService_GetParticipationsByProjectIDAsync_ShouldReturnUsersParticipations()
    {
        //Arange
        var role1 = RoleFactory.OwnerRole();
        var role2 = RoleFactory.DevRole();
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = UserFactory.Create("test@test.com");
        var user2 = UserFactory.Create("test2@test.com");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = ProjectFactory.Create(user1);

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var participationOwner = new Participation
            (
                project,
                role1,
                user1
            );
        var nonOwnerParticipation = new Participation
            (
                project,
                role2,
                user2
            );

        await _context.Participations.AddRangeAsync(participationOwner, nonOwnerParticipation);
        await _context.SaveChangesAsync();

        var participationGet1 = new ParticipationGET(
            project.ProjectId,
            new DTOs.User.UserParticipationGET(
                user1.UserId,
                user1.FirstName,
                user1.LastName,
                user1.Email,
                new DTOs.Role.RoleGET(role1.RoleId, role1.Name)
            )
        );
        var participationGet2 = new ParticipationGET(
            project.ProjectId,
            new DTOs.User.UserParticipationGET(
                user2.UserId,
                user2.FirstName,
                user2.LastName,
                user2.Email,
                new DTOs.Role.RoleGET(role2.RoleId, role2.Name)
            )
        );
        //Act
        var participations = await _participationService.GetParticipationsByProjectIDAsync(project.ProjectId);
        //Assert
        participations.Should()
            .HaveCount(_context.Participations.Where(p => p.ProjectId == project.ProjectId).Count())
            .And.ContainEquivalentOf(participationGet1)
            .And.ContainEquivalentOf(participationGet2);
    }
}
