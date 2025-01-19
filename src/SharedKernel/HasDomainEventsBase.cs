using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel;

public abstract class HasDomainEventsBase : IHasDomainEvents
{
    private readonly List<DomainEventBase> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEventBase eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    protected void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RemoveDomainEvent(DomainEventBase eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }
}
