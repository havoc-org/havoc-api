using Microsoft.AspNetCore.Diagnostics;
using WarehouseApp2.Exceptions;

namespace WarehouseApp2.Middlewares;

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
                Message=$"Domain exception: {exceptionMessage}, Time of occurrence {DateTime.UtcNow}"
            }, cancellationToken: cancellationToken);
            return true;
        }

        return false;
    }
}