using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ModularMonolith.Web.Configuration;

public class MessageBrokerSettings
{
    internal const string SectionName = "MessageBroker";

    [Required]
    public string Host { get; set; } = string.Empty;

    [Required]
    public string Port { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

[OptionsValidator]
public partial class ValidateMessageBrokerSettings : IValidateOptions<MessageBrokerSettings>;
