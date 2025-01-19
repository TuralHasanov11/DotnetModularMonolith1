using MediatR;

namespace SharedKernel;

public abstract record DomainEventBase : INotification
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
