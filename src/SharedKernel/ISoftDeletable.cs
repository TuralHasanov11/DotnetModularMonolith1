namespace SharedKernel;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }

    public DateTime DeletedOnUtc { get; set; }

    void Delete();
}
