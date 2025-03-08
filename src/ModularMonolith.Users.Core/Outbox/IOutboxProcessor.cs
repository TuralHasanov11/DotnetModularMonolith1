namespace ModularMonolith.Users.Core.Outbox;

public interface IOutboxProcessor
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
