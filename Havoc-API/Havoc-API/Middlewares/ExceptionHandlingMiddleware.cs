using Havoc_API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Havoc_API.Middlewares;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;
    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        this._logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is DomainException)
        {
            var exceptionMessage = exception.Message;
            _logger.LogError(
                "Domain exception: {exceptionMessage}, Time of occurrence {time}",
                exceptionMessage, DateTime.UtcNow);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Message = $"Domain exception: {exceptionMessage}, Time of occurrence {DateTime.UtcNow}"
            }, cancellationToken: cancellationToken);
            return true;
        }
        else if (exception is NotFoundException)
        {
            var exceptionMessage = exception.Message;
            _logger.LogError(
                "Not found exception: {exceptionMessage}, Time of occurrence {time}",
                exceptionMessage, DateTime.UtcNow);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Message = $"Not found exception: {exceptionMessage}, Time of occurrence {DateTime.UtcNow}"
            }, cancellationToken: cancellationToken);
            return true;
        }
        else if (exception is DataAccessException)
        {
            var exceptionMessage = exception.Message;
            _logger.LogError(
                "Data access exception: {exceptionMessage}, Time of occurrence {time}",
                exceptionMessage, DateTime.UtcNow);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Message = $"Data access exception: {exceptionMessage}, Time of occurrence {DateTime.UtcNow}"
            }, cancellationToken: cancellationToken);
            return true;
        }

        var exceptionMsg = exception.Message;
        _logger.LogError(
            "Internal exception: {exceptionMessage}, Time of occurrence {time}",
            exceptionMsg, DateTime.UtcNow);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            Message = $"Internal exception: {exceptionMsg}, Time of occurrence {DateTime.UtcNow}"
        }, cancellationToken: cancellationToken);
        return true;
    }
}