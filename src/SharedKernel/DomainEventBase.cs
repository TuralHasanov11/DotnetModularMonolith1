using MediatR;

namespace SharedKernel;

public abstract record DomainEventBase(DateTime OccurredOnUtc) : INotification;
