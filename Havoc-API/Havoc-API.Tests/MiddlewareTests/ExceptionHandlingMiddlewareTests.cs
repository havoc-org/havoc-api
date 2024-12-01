using System.Data;
using System.Reflection;
using FluentAssertions;
using Havoc_API.Exceptions;
using Havoc_API.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Havoc_API.Tests.MiddlewareTests;

public class SupportedExceptionTestData : TheoryData<SupportedException, int>
{
    private int badRequestCode = 400;
    private int notFoundCode = 404;
    private int internalErrorCode = 500;
    public SupportedExceptionTestData()
    {
        Add(new DomainException("This is not correct"), badRequestCode);
        Add(new DomainException("sdlkjgf;lskdjf;skjdgflsdkjgskdjg"), badRequestCode);
        Add(new DomainException("Logic is correct, you -- are not"), badRequestCode);

        Add(new NotFoundException("No data"), notFoundCode);
        Add(new NotFoundException("U sure u loookin' fo' it, boah?"), notFoundCode);
        Add(new NotFoundException("Nuh uuuuuuuuuuuuh"), notFoundCode);

        Add(new DataAccessException("Database BOOM"), internalErrorCode);
        Add(new DataAccessException("SQl doesnt work"), internalErrorCode);
        Add(new DataAccessException("AAAAAAAAAAAAAAA"), internalErrorCode);
    }
}

public class NonSupportedExceptionTestData : TheoryData<Exception>
{
    public NonSupportedExceptionTestData()
    {
        Add(new ArgumentNullException("This is not correct"));
        Add(new ConstraintException("sdlkjgf;lskdjf;skjdgflsdkjgskdjg"));
        Add(new NotImplementedException("Logic is correct, you -- are not"));

        Add(new OutOfMemoryException("No data"));
        Add(new DbUpdateException("U sure u loookin' fo' it, boah?"));
        Add(new DbUpdateConcurrencyException("Nuh uuuuuuuuuuuuh"));

        Add(new Exception("Database BOOM"));
        Add(new ReadOnlyException("SQl doesnt work"));
        Add(new CustomAttributeFormatException("AAAAAAAAAAAAAAA"));
    }
}

public class CustomExceptionHandlerTests
{
    private readonly CustomExceptionHandler _exceptionHandler;
    private readonly Mock<ILogger<CustomExceptionHandler>> _logger;
    public CustomExceptionHandlerTests()
    {
        _logger = new Mock<ILogger<CustomExceptionHandler>>();

        //SUT
        _exceptionHandler = new CustomExceptionHandler(_logger.Object);
    }

    [Theory]
    [ClassData(typeof(SupportedExceptionTestData))]
    public async void TryHandleAsync_ChangesResponseToCorrespondingCodeAndLogsExceptionAndReturnsTrue_WhenSupportedExceptionIsThrown(SupportedException supportedException, int statusCode)
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var cancellationToken = It.IsAny<CancellationToken>();
        //Act
        var handling = await _exceptionHandler.TryHandleAsync(httpContext, supportedException, cancellationToken);
        handling.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(statusCode);
        _logger.Verify
        (
            x => x.Log
                (
                    LogLevel.Error,  // Specifically checking for Error level
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ),
            Times.Once()
        );
    }

    [Theory]
    [ClassData(typeof(NonSupportedExceptionTestData))]
    public async void TryHandleAsync_ChangeResponseToInternalErrorAndLogsItAndReturnsTrue_WhenNotSupportedExceptionIsThrown(Exception nonSupportedException)
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var cancellationToken = It.IsAny<CancellationToken>();
        //Act
        var handling = await _exceptionHandler.TryHandleAsync(httpContext, nonSupportedException, cancellationToken);
        handling.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(500);
        _logger.Verify
        (
            x => x.Log
                (
                    LogLevel.Error,  // Specifically checking for Error level
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ),
            Times.Once()
        );
    }
}