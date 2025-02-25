using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace ModularMonolith.Users.Infrastructure;

public class EmailProcessor(
    Channel<EmailRequest> channel,
    IEmailSender emailSender,
    ILogger<EmailProcessor> logger) : BackgroundService
{
    private readonly Channel<EmailRequest> _channel = channel;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<EmailProcessor> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _channel.Reader.WaitToReadAsync(stoppingToken))
        {
            var request = await _channel.Reader.ReadAsync(stoppingToken);

            await _emailSender.SendEmailAsync(request.Email, request.Subject, request.Message);

            _logger.LogVerificationEmailSent(request.Email);
        }

        _logger.LogVerificationEmailProcessorStopped();
    }
}

public static partial class LogEmailVerification
{
    [LoggerMessage(1, LogLevel.Information, "Verification email sent to {Email}")]
    public static partial void LogVerificationEmailSent(this ILogger<EmailProcessor> logger, string email);

    [LoggerMessage(2, LogLevel.Information, "Verification email processor stopped")]
    public static partial void LogVerificationEmailProcessorStopped(this ILogger<EmailProcessor> logger);
}
