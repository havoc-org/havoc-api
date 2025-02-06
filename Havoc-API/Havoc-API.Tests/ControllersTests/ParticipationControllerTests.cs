using System.Threading.Tasks;
using FluentAssertions;
using Havoc_API.Controllers;
using Havoc_API.DTOs.Participation;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ControllersTests;
public class ParticipationControllerTests
{
    private readonly Mock<IParticipationService> _participationService;
    private readonly Mock<IUserService> _userService;
    private readonly ParticipationController _participationController;
    public ParticipationControllerTests()
    {
        _userService = new Mock<IUserService>();
        _participationService = new Mock<IParticipationService>();
        _participationController = new ParticipationController(_participationService.Object, _userService.Object);
    }

    [Fact]
    public async void RemoveParticipation_ReturnNoContentResult_WhenParticipationWasRemoved()
    {
        //Arrange
        _userService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(It.IsAny<int>);
        _participationService.Setup(service => service.GetUserRoleInProjectAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(RoleFactory.OwnerRole());

        _participationService.Setup(service => service.DeleteParticipation(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(It.IsAny<int>());

        //Act
        var result = await _participationController.RemoveParticipation(It.IsAny<int>(), It.IsAny<int>());

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async void AddParticipations_ShouldReturnOkObjectResultWithTrue_WhenParticipationWasAddedSuccessfully()
    {
        //Arrange
        var participations = new List<ParticipationPOST>()
        {
            ParticipationFactory.CreatePost(email: "ababa@bam.com"),
            ParticipationFactory.CreatePost(email: "kaboom@brooklyn.com", roleType: RoleType.Manager),
            ParticipationFactory.CreatePost(roleType: RoleType.Owner),
            ParticipationFactory.CreatePost(email: "kalabanga@porto.pl")
        };
        _userService.Setup(service => service.GetUserId(It.IsAny<HttpRequest>())).Returns(It.IsAny<int>);
        _participationService.Setup(service => service.GetUserRoleInProjectAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(RoleFactory.OwnerRole());

        _participationService.Setup(service => service.AddParticipationListAsync(participations)).ReturnsAsync(true);

        //Act
        var result = await _participationController.AddParticipations(participations, It.IsAny<int>());

        //Assert
        result.Should().BeOfType<OkObjectResult>().Which.Value.As<bool>().Should().BeTrue();
    }
}