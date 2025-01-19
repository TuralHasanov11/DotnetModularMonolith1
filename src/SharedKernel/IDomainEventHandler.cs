using MediatR;

namespace SharedKernel;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : DomainEventBase;
