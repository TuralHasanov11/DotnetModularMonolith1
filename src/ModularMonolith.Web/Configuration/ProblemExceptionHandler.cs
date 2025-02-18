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
            Status = exception switch
            {
                _ => StatusCodes.Status500InternalServerError
            },
            Title = exception.Message,
            Detail = exception.Message,
            Type = exception.GetType().Name
        };

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
            });
    }
}


public static partial class ProblemExceptionHandlerLogger
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Error Message: {Message}, Time of occurrence {Time}")]
    public static partial void LogProblemException(this ILogger<ProblemExceptionHandler> logger, string message, DateTime time);
}
