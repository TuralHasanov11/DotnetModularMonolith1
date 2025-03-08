namespace SharedKernel;

public interface IAuditable
{
    DateTime CreatedOnUtc { get; }

    DateTime? UpdatedOnUtc { get; }

    string? CreatedByUserId { get; }

    string? CreatedByUserName { get; }

    string? UpdatedByUserId { get; }

    string? UpdatedByUserName { get; }
}

public abstract class AuditableEntityBase : EntityBase, IAuditable
{
    public DateTime CreatedOnUtc { get; }

    public string? CreatedByUserId { get; }

    public string? CreatedByUserName { get; }

    public DateTime? UpdatedOnUtc { get; }

    public string? UpdatedByUserId { get; }

    public string? UpdatedByUserName { get; }
}


public abstract class AuditableEntityBase<TId> : EntityBase<TId>, IAuditable
    where TId : struct, IEquatable<TId>
{
    public DateTime CreatedOnUtc { get; }

    public string? CreatedByUserId { get; }

    public string? CreatedByUserName { get; }

    public DateTime? UpdatedOnUtc { get; }

    public string? UpdatedByUserId { get; }

    public string? UpdatedByUserName { get; }
}
