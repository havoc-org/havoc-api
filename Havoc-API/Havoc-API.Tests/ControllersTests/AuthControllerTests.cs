using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Havoc_API.Controllers;
using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Models;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ControllersTests;

public class AuthControllerTests
{
    //DO NOT TOUCH
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IUserService> _userService;
    private readonly IConfiguration _config;
    private readonly AuthController _authController;
    private readonly Mock<HttpContext> _httpContext;
    private readonly Mock<IRequestCookieCollection> _httpContextRequestCookies;
    private readonly Mock<IResponseCookies> _httpContextResponseCookies;

    public AuthControllerTests()
    {
        _httpContextRequestCookies = new Mock<IRequestCookieCollection>();
        _httpContextResponseCookies = new Mock<IResponseCookies>();
        _httpContext = new Mock<HttpContext>();
        _httpContext.SetupGet(x => x.Request.Cookies).Returns(_httpContextRequestCookies.Object);
        _httpContext.Setup(x => x.Response.Cookies).Returns(_httpContextResponseCookies.Object);

        var inMemorySettings = new[]
        {
            new KeyValuePair<string, string?>("JWT:Key", "test_key_should_be_longer_but_idk_if_that_will_be_enough"),
            new KeyValuePair<string, string?>("JWT:Issuer", "TestIssuer"),
            new KeyValuePair<string, string?>("JWT:Audience", "TestAudience")
        };
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        _tokenService = new Mock<ITokenService>();
        _userService = new Mock<IUserService>();
        _authController = new AuthController
            (
                tokenService: _tokenService.Object,
                userService: _userService.Object,
                configuration: _config
            )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext.Object
            }
        };
    }

    [Fact]
    public async void RegisterUserAsync_ReturnsOkResult_WhenUserIsRegistered()
    {
        // Arrange
        var user = UserFactory.CreatePost();
        _userService.Setup(x => x.AddUserAsync(user)).ReturnsAsync(true);

        // Act
        var result = await _authController.RegisterUserAsync(user);

        // Assert
        result
            .Should().BeOfType<OkObjectResult>()
            .Which.Value.Should()
            .BeEquivalentTo(new { message = "User registered successfully!" });
    }

    [Fact]
    public void LoginUserAsync_ShouldReturnOkResultWithAccessToken_WhenUserIsVerified()
    {
        // Arrange
        var userModel = UserFactory.Create();
        var user = new UserLogin() { Email = userModel.Email, Password = userModel.Password };
        var userToken = new UserToken(It.IsAny<int>(), userModel.FirstName, userModel.LastName, userModel.Email);
        _httpContextResponseCookies.Setup(x => x.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()));
        _userService.Setup(x => x.VerifyUserAsync(user)).ReturnsAsync(userToken);
        _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(userModel);
        _tokenService.Setup(x => x.GenerateAccessToken(userToken)).Returns("test_token");
        _tokenService.Setup(x => x.GenerateRefreshToken(userModel.UserId)).Returns("refresh_token");

        // Act
        var result = _authController.LoginUserAsync(user).Result;

        // Assert
        result
            .Should().BeOfType<OkObjectResult>()
            .Which.Value.Should()
            .BeEquivalentTo(new { accessToken = "test_token" });
    }

    [Fact]
    public void RefreshToken_ShouldReturnOkResultWithAccessToken_WhenRefreshTokenIsValid()
    {
        // Arrange
        var userId = It.IsAny<int>();
        var user = UserFactory.Create();
        var oldRefreshToken = "test_old_refresh_token";
        var newRefreshToken = "test_new_refresh_token";
        var accessToken = "test_access_token";

        var claimsPrincipal = new Mock<ClaimsPrincipal>();
        claimsPrincipal.Setup(x => x.Claims).Returns(new List<Claim> { new Claim("UserId", "1") });

        var userToken = new UserToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        _httpContextRequestCookies.Setup(x => x["RefreshToken"]).Returns(oldRefreshToken);
        _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
        _tokenService.Setup(x => x.ValidateRefreshToken(oldRefreshToken)).Returns(claimsPrincipal.Object);
        _tokenService.Setup(x => x.GenerateAccessToken(It.IsAny<UserToken>())).Returns(accessToken);
        _tokenService.Setup(x => x.GenerateRefreshToken(It.IsAny<int>())).Returns(newRefreshToken);

        // Act
        var result = _authController.RefreshToken();

        // Assert   
        result
            .Should().BeOfType<OkObjectResult>()
            .Which.Value.Should()
            .BeEquivalentTo
            (
                new
                {
                    accessToken = accessToken,
                    userId = user.UserId,
                    email = user.Email
                }
            );
    }
}
