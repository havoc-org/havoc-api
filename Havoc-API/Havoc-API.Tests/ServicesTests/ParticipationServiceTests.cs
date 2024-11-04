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
        var role1 = new Role(RoleType.Manager);
        var role2 = new Role(RoleType.Developer);
        await _context.Roles.AddRangeAsync(role1, role2);

        var user1 = new User("Test", "Test", "test@test.test", "test");
        var user2 = new User("Test2", "Test2", "test2@test.test", "test");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = new Project
           (
               "Test",
               "test",
               new byte[1234],
               DateTime.Now,
               DateTime.Now.AddDays(34),
               user1,
               new ProjectStatus("test")
           );

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
        var role1 = new Role(RoleType.Manager);
        var role2 = new Role(RoleType.Developer);
        await _context.Roles.AddRangeAsync(role1, role2);

        _context.Users.RemoveRange(_context.Users.ToList());
        var user1 = new User("Test", "Test", "test@test.test", "test");
        var user2 = new User("Test2", "Test2", "test2@test.test", "test");
        await _context.Users.AddRangeAsync(user1, user2);

        var project = new Project
           (
               "Test",
               "test",
               new byte[1234],
               DateTime.Now,
               DateTime.Now.AddDays(34),
               user1,
               new ProjectStatus("test")
           );

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
}
