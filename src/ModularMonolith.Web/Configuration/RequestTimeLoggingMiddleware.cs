using System.Diagnostics;

namespace ModularMonolith.Web.Configuration;

public partial class RequestTimeLoggingMiddleware(ILogger<RequestTimeLoggingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<RequestTimeLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var startTime = Stopwatch.GetTimestamp();
        LogRequestStarted(_logger);

        await next(context);

        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        LogRequestFinished(_logger, elapsedTime);
    }

    [LoggerMessage(LogLevel.Information, "Request started")]
    public static partial void LogRequestStarted(
        ILogger<RequestTimeLoggingMiddleware> logger);

    [LoggerMessage(LogLevel.Information, "Request finished in {ElapsedTime}")]
    public static partial void LogRequestFinished(
        ILogger<RequestTimeLoggingMiddleware> logger,
        TimeSpan elapsedTime);
}
