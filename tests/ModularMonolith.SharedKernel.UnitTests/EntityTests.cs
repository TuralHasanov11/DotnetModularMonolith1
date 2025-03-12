namespace ModularMonolith.SharedKernel.UnitTests;

public class EntityTests
{
    private record TestDomainEvent(DateTime OccurredOnUtc) : DomainEventBase(OccurredOnUtc);

    private class TestEntity : HasDomainEventsBase
    {
        public void AddEvent(DomainEventBase eventItem)
        {
            AddDomainEvent(eventItem);
        }

        public void ClearEvents()
        {
            ClearDomainEvents();
        }

        public void RemoveEvent(DomainEventBase eventItem)
        {
            RemoveDomainEvent(eventItem);
        }
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToDomainEvents()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent(DateTime.UtcNow);

        // Act
        entity.AddEvent(domainEvent);

        // Assert
        Assert.Contains(domainEvent, entity.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent1 = new TestDomainEvent(DateTime.UtcNow);
        var domainEvent2 = new TestDomainEvent(DateTime.UtcNow.AddMinutes(1));
        entity.AddEvent(domainEvent1);
        entity.AddEvent(domainEvent2);

        // Act
        entity.ClearEvents();

        // Assert
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldRemoveSpecificEvent()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent1 = new TestDomainEvent(DateTime.UtcNow);
        var domainEvent2 = new TestDomainEvent(DateTime.UtcNow.AddMinutes(1));
        entity.AddEvent(domainEvent1);
        entity.AddEvent(domainEvent2);

        // Act
        entity.RemoveEvent(domainEvent1);

        // Assert
        Assert.DoesNotContain(domainEvent1, entity.DomainEvents);
        Assert.Contains(domainEvent2, entity.DomainEvents);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldNotThrow_WhenEventNotInList()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent1 = new TestDomainEvent(DateTime.UtcNow);
        var domainEvent2 = new TestDomainEvent(DateTime.UtcNow.AddMinutes(1));
        entity.AddEvent(domainEvent1);

        // Act & Assert
        var exception = Record.Exception(() => entity.RemoveEvent(domainEvent2));
        Assert.Null(exception);
    }
}
