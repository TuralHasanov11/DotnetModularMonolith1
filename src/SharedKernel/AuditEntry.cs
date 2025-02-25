namespace SharedKernel;

public class AuditEntry
{
    public Guid Id { get; set; }

    public string Metadata { get; set; } = string.Empty;

    public DateTime StartTimeUtc { get; set; }

    public DateTime EndTimeUtc { get; set; }

    public bool Succeeded { get; set; }

    public string? ErrorMessage { get; set; } = string.Empty;
}


public record AuditTrailMessage
{
    public List<AuditEntry> Entries { get; set; } = [];
}
