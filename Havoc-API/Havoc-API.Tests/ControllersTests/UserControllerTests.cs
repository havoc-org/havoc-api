using System;
using System.Threading.Tasks;
using FluentAssertions;
using Havoc_API.Controllers;
using Havoc_API.DTOs.User;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ControllersTests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _userController;
    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _userController = new UserController(_mockUserService.Object);
    }

    [Fact]
    public async void GetUserAsync_ShouldReturnOKWithUser_WhenUserExists()
    {
        //Arrange
        var userId = 1;
        var user = UserFactory.Create();
        _mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        //Act
        var result = await _userController.GetUserAsync(userId);

        //Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new { user });
    }

    [Fact]
    public async void UpdateUserAsync_ShoudlUpdateUserData_AndReturnNumberOfAffectedRows_WhenUserExists()
    {
        //Arrange
        var userId = 1;
        int rows = 20;
        var user = new UserPATCH()
        {
            UserId = userId,
            Name = "Test",
            SurName = "Testing"
        };
        _mockUserService.Setup(x => x.UpdateUserAsync(user)).ReturnsAsync(rows);

        //Act
        var result = await _userController.UpdateUserAsync(user);

        //Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new { AffectedRows = rows });
    }
}
