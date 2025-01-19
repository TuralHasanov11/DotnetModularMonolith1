namespace SharedKernel;

public abstract class EntityBase : HasDomainEventsBase
{
    public int Id { get; }

    protected EntityBase() { }

    public byte[]? RowVersion { get; }
}

public abstract class EntityBase<TId> : HasDomainEventsBase
  where TId : struct, IEquatable<TId>
{
    protected EntityBase() { }

    public TId Id { get; } = default!;
}
