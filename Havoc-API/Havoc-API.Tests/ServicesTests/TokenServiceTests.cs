using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Havoc_API.DTOs.Tokens;
using Havoc_API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace Havoc_API.Tests.ServicesTests;

public class TokenServiceTests
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;
    public TokenServiceTests()
    {
        var inMemorySettings = new[]
        {
            new KeyValuePair<string, string?>("JWT:Key", "test_key_should_be_longer_but_idk_if_that_will_be_enough"),
            new KeyValuePair<string, string?>("JWT:Issuer", "TestIssuer"),
            new KeyValuePair<string, string?>("JWT:Audience", "TestAudience")
        };
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        _tokenService = new TokenService(_config);
    }

    [Fact]
    public void TokenService_GenerateAccessToken_ShouldReturnAccessTokenForOneHourWithUserClaims()
    {
        //Arrange
        var userToken = new UserToken(1, "Test", "Test", "test@test.test");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
        var rules = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        //Act
        var token = _tokenService.GenerateAccessToken(userToken);
        var tokenObject = tokenHandler.ReadJwtToken(token);
        var claimsPrincipal = tokenHandler.ValidateToken(token, rules, out SecurityToken validatedToken);

        //Assert
        tokenObject.Issuer.Should().Be(_config["JWT:Issuer"]);
        tokenObject.Audiences.Should().Contain(_config["JWT:Audience"]);
        tokenObject.ValidTo.Should()
            .BeOnOrBefore(DateTime.UtcNow.AddHours(1).AddMinutes(1))
            .And
            .BeAfter(DateTime.UtcNow.AddHours(1).AddMinutes(-1));

        claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value
            .Should().Be(userToken.Id + "");
        claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            .Should().BeOfType<string>();
    }

    [Fact]
    public void TokenService_GenerateRefreshToken_ShouldReturnRefreshTokenForOneHour()
    {
        //Arrange
        var userId = 1;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
        var rules = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        //Act
        var token = _tokenService.GenerateRefreshToken(userId);
        var tokenObject = tokenHandler.ReadJwtToken(token);
        var claimsPrincipal = tokenHandler.ValidateToken(token, rules, out SecurityToken validatedToken);

        //Assert
        tokenObject.Issuer.Should().Be(_config["JWT:Issuer"]);
        tokenObject.Audiences.Should().Contain(_config["JWT:Audience"]);
        tokenObject.ValidTo.Should()
            .BeOnOrBefore(DateTime.UtcNow.AddDays(3).AddMinutes(1))
            .And
            .BeAfter(DateTime.UtcNow.AddDays(3).AddMinutes(-1));

        claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value
            .Should().Be(userId + "");
    }

    [Fact]
    public void TokenService_ValidateRefreshToken_ShouldReturnClaims_WhenRefrshTokenIsValid()
    {
        //Arrange
        var userId = 1;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
        var rules = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        var claims = new List<Claim>
            {
                new Claim("UserId",userId.ToString()),
                // Add more claims as necessary
            };

        var token = new JwtSecurityToken(
           issuer: _config["JWT:Issuer"],
           audience: _config["JWT:Audience"],
           claims: claims,
           expires: DateTime.UtcNow.AddDays(3),
           signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature));

        var refeshToken = tokenHandler.WriteToken(token);

        //Act
        var validationResult = _tokenService.ValidateRefreshToken(refeshToken);
        var claimsPrincipal = tokenHandler.ValidateToken(refeshToken, rules, out SecurityToken validatedToken);

        validationResult
            .Should().NotBeNull();
        validationResult?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value
            .Should().Be(userId + "");
    }

    [Fact]
    public void TokenService_ValidateRefreshToken_ShouldReturnNull_WhenRefrshTokenIsNotValid()
    {
        //Arrange
        var userId = 1;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
        var rules = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        var claims = new List<Claim>
            {
                new ("UserId",userId.ToString()),
            };

        var token = new JwtSecurityToken(
           issuer: _config["JWT:Issuer"],
           audience: _config["JWT:Audience"],
           claims: claims,
           expires: DateTime.UtcNow,
           signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature));

        var refeshToken = tokenHandler.WriteToken(token);

        //Act
        var validationResult = _tokenService.ValidateRefreshToken(refeshToken);

        validationResult
            .Should().BeNull();
    }
}
