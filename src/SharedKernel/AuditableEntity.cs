namespace SharedKernel;

public abstract class AuditableEntity : EntityBase, IAuditable
{
    public DateTime CreatedOnUtc { get; }

    public string? CreatedByUserId { get; }

    public string? CreatedByUserName { get; }

    public DateTime? UpdatedOnUtc { get; }

    public string? UpdatedByUserId { get; }

    public string? UpdatedByUserName { get; }
}
