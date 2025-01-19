using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ModularMonolith.Web.Configuration;

public class ProblemExceptionHandler(
    ILogger<ProblemExceptionHandler> logger,
    IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    private readonly ILogger<ProblemExceptionHandler> _logger = logger;

    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogProblemException(exception.Message, DateTime.UtcNow);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Detail = exception.Message,
            Type = "Server Error",
        };

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
            });
    }
}

public static partial class ProblemExceptionLogger
{
    [LoggerMessage(LogLevel.Error, "Error Message: {Message}, Time of occurrence {Time}")]
    public static partial void LogProblemException(this ILogger logger, string message, DateTime time);
}