using System.Diagnostics;

namespace ModularMonolith.Web.Configuration;


public class RequestTimeLoggingMiddleware(ILogger<RequestTimeLoggingMiddleware> logger) : IMiddleware
{
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var startTime = Stopwatch.GetTimestamp();
        _logger.LogRequestStarted();

        await next(context);

        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogRequestFinished(elapsedTime);
    }
}

public static partial class RequestTimeLogger
{
    [LoggerMessage(LogLevel.Information, "Request started")]
    public static partial void LogRequestStarted(this ILogger logger);
}

public static partial class RequestTimeLogger
{
    [LoggerMessage(LogLevel.Information, "Request finished in {ElapsedTime}")]
    public static partial void LogRequestFinished(this ILogger logger, TimeSpan elapsedTime);
}