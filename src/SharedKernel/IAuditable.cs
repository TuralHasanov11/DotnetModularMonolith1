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
