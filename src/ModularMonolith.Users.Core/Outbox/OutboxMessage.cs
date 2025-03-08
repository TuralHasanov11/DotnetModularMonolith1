using System.Text.Json;

namespace ModularMonolith.Users.Core.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage(string type, string content, DateTime createdOnUtc)
    {
        Id = Guid.CreateVersion7();
        Type = type;
        Content = content;
        CreatedOnUtc = createdOnUtc;
    }

    private OutboxMessage() { }

    public Guid Id { get; }

    public string Type { get; }

    public string Content { get; }

    public DateTime CreatedOnUtc { get; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public void Process(DateTime processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
    }

    public void Fail(string error, DateTime processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
        Error = error;
    }

    public static OutboxMessage Create<T>(T message, DateTime createdOnUtc)
        where T : notnull
    {
        return new(message.GetType().FullName!, JsonSerializer.Serialize(message), createdOnUtc);
    }
}
