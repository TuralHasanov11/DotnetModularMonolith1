using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ModularMonolith.Users.Infrastructure;

public class EmailSender(IFluentEmail fluentEmail) : IEmailSender
{
    private readonly IFluentEmail _fluentEmail = fluentEmail;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await _fluentEmail.To(email).Subject(subject).Body(htmlMessage).SendAsync();
    }
}
