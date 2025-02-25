using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel;

namespace ModularMonolith.Users.Infrastructure.Data;

public class AuditInterceptor(List<AuditEntry> auditEntryList, IPublishEndpoint publishEndpoint) : SaveChangesInterceptor
{
    private readonly List<AuditEntry> _auditEntries = auditEntryList;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var startTime = DateTime.UtcNow;

            var auditEntries = eventData.Context.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is not AuditEntry &&
                    (e.State == EntityState.Added
                        || (e.State == EntityState.Modified || e.References.Any(r => r.TargetEntry?.Metadata.IsOwned() == true &&
                                    (r.TargetEntry.State == EntityState.Added ||
                                    r.TargetEntry.State == EntityState.Modified)))))
                .Select(x => new AuditEntry
                {
                    Id = Guid.CreateVersion7(),
                    StartTimeUtc = startTime,
                    Metadata = x.DebugView.LongView,
                }).ToList();

            if (auditEntries.Count != 0)
            {
                _auditEntries.AddRange(auditEntries);
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var endTime = DateTime.UtcNow;

            foreach (var item in _auditEntries)
            {
                item.EndTimeUtc = endTime;
                item.Succeeded = true;
            }

            if (_auditEntries.Count > 0)
            {
                _auditEntries.Clear();
                await _publishEndpoint.Publish(new AuditTrailMessage { Entries = _auditEntries }, cancellationToken);
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override async Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var endTime = DateTime.UtcNow;

            foreach (var item in _auditEntries)
            {
                item.EndTimeUtc = endTime;
                item.Succeeded = false;
                item.ErrorMessage = eventData.Exception.Message;
            }

            if (_auditEntries.Count > 0)
            {
                _auditEntries.Clear();
                await _publishEndpoint.Publish(new AuditTrailMessage { Entries = _auditEntries }, cancellationToken);
            }
        }
    }
}

