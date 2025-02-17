using System;
using System.Threading.Tasks;
using FluentAssertions;
using Havoc_API.Data;
using Havoc_API.DTOs.User;
using Havoc_API.Services;
using Havoc_API.Tests.TestData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ServicesTests;

public class UserServiceTests
{
    private readonly IHavocContext _context;
    private readonly IUserService _userService;
    public UserServiceTests()
    {
        _context = HavocTestContextFactory.GetTestContext();
        _userService = new UserService(_context);
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUserAndReturnTrue_WhenUserEmailDoesntExist()
    {
        // Arrange
        var user = UserFactory.CreatePost();

        var check = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        if (check is not null)
            _context.Users.Remove(check);
        var userCount = _context.Users.Count();

        // Act
        var result = await _userService.AddUserAsync(user);

        // Assert
        _context.Users.Should().HaveCount(userCount + 1);
        _context.Users.FirstOrDefault(u => u.Email == user.Email).Should().NotBeNull();
        result.Should().BeTrue();
    }

    [Fact]
    public async void GetUserId_ShouldReturnUserId_WhenTokenIsValid()
    {
        //Arrange
        var user = UserFactory.Create();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var inMemorySettings = new[]
        {
            new KeyValuePair<string, string?>("JWT:Key", "test_key_should_be_longer_but_idk_if_that_will_be_enough"),
            new KeyValuePair<string, string?>("JWT:Issuer", "TestIssuer"),
            new KeyValuePair<string, string?>("JWT:Audience", "TestAudience")
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var tokenService = new TokenService(config);
        var token = tokenService.GenerateAccessToken
        (
            new DTOs.Tokens.UserToken
                (
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email
                )
        );

        var headers = new HeaderDictionary
        {
            { "Authorization", "Bearer "+token}
        };
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(r => r.Headers).Returns(headers);
        //Act
        var result = _userService.GetUserId(mockRequest.Object);

        //Assert
        result.Should().Be(user.UserId);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = UserFactory.Create();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetUserByIdAsync(user.UserId);

        // Assert
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async void UpdateUserAsync_ShouldUpdateUserAndReturnrNumberOfAffectedLines_WhenUserExists()
    {
        // Arrange
        var user = UserFactory.Create();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var updatedUser = new UserPATCH
        {
            UserId = user.UserId,
            Name = "UpdatedName",
            SurName = "UpdatedLastName"
        };

        // Act
        var result = await _userService.UpdateUserAsync(updatedUser);

        // Assert
        result.Should().BePositive();
        _context.Users
        .Select(u => new UserPATCH() { UserId = u.UserId, Name = u.FirstName, SurName = u.LastName })
        .FirstOrDefault(u => u.UserId == user.UserId)
        .Should()
        .BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async void VerifyEmailAsync_ShouldReturnTrue_WhenUserWithThatEmailExists()
    {
        // Arrange
        var user = UserFactory.Create();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.VerifyEmailAsync(user.Email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async void VerifyUserAsync_ShouldReturnUserTokenInstanceBasedOnUseerProfile_WhenCredentialsAreValid()
    {
        // Arrange
        var pwd = "testPass";
        var user = UserFactory.Create("test@gmail.com", BCrypt.Net.BCrypt.HashPassword(pwd));
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var userLogin = new UserLogin
        {
            Email = user.Email,
            Password = pwd
        };

        // Act
        var result = await _userService.VerifyUserAsync(userLogin);

        // Assert
        result.Should().BeEquivalentTo
        (
            new DTOs.Tokens.UserToken
            (
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email
            )
        );
    }

    [Fact]
    public async void UpdateUserPasswordAsync_ShouldReturnNumbrOfAffectedLines_WhenUpdateWasSuccessful()
    {
        // Arrange
        var pwd = "testPass";
        var newPwd = "newPassword";
        var user = UserFactory.Create("test@gmail.com", BCrypt.Net.BCrypt.HashPassword(pwd));
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        //Act
        var result = await _userService.UpdateUserPasswordAsync(user.UserId, pwd, newPwd);

        //Arrange
        result.Should().BePositive();
        BCrypt.Net.BCrypt.Verify(newPwd, user.Password).Should().BeTrue();
    }

    [Fact]
    public async void GetUserGETByIdAsync_ShouldReturnUserGET_WhenUserWithSuchIdExists()
    {
        // Arrange
        var user = UserFactory.Create();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var userGet = UserFactory.CreateGet(user);
        // Act
        var result = await _userService.GetUserGETByIdAsync(user.UserId);

        // Assert
        result.Should().BeEquivalentTo(userGet);
    }

}