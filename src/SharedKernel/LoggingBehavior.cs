using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SharedKernel;

public class LoggingBehavior<TRequest, TResponse>(ILogger<Mediator> logger)
    : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    private readonly ILogger<Mediator> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogHandlingRequest(typeof(TRequest).Name);

        var timestamp = Stopwatch.GetTimestamp();

        var response = await next();

        _logger.LogHandledRequest(
            typeof(TRequest).Name,
            typeof(TResponse).Name,
            Stopwatch.GetTimestamp() - timestamp);

        return response;
    }
}

public static partial class LoggingBehaviorLogger
{
    [LoggerMessage(LogLevel.Information, "Handling {RequestName}")]
    public static partial void LogHandlingRequest(this ILogger<Mediator> logger, string requestName);

    [LoggerMessage(LogLevel.Information, "Handled {RequestName} with {ResponseName} in {ElapsedTime} ms")]
    public static partial void LogHandledRequest(
        this ILogger<Mediator> logger,
        string requestName,
        string responseName,
        long elapsedTime);
}
