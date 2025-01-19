using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SharedKernel;

public class LoggingBehavior<TRequest, TResponse>(ILogger<Mediator> logger)
    : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    private readonly ILogger<Mediator> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        }

        var timestamp = Stopwatch.GetTimestamp();

        var response = await next();

        _logger.LogInformation(
            "Handled {RequestName} with {Response} in {ElapsedTime} ms",
            typeof(TRequest).Name,
            response,
            Stopwatch.GetElapsedTime(timestamp));

        return response;
    }
}
