using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel;

namespace ModularMonolith.Infrastructure.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var entries = eventData.Context.ChangeTracker
                .Entries<AuditableEntity>()
                .Where(e =>
                    e.State == EntityState.Added ||
                    (e.State == EntityState.Modified ||
                    e.References.Any(r => r.TargetEntry?.Metadata.IsOwned() == true &&
                                    (r.TargetEntry.State == EntityState.Added ||
                                    r.TargetEntry.State == EntityState.Modified))));

            foreach (var entity in entries)
            {
                if (entity.State is EntityState.Added)
                {
                    entity.Property(e => e.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
                    entity.Property(e => e.UpdatedOnUtc).CurrentValue = DateTime.UtcNow;
                    entity.Property(e => e.CreatedByUserId).CurrentValue = "";
                    entity.Property(e => e.UpdatedByUserId).CurrentValue = "";
                    entity.Property(e => e.CreatedByUserName).CurrentValue = "";
                    entity.Property(e => e.UpdatedByUserName).CurrentValue = "";
                }
                else
                {
                    entity.Property(e => e.UpdatedOnUtc).CurrentValue = DateTime.UtcNow;
                    entity.Property(e => e.UpdatedByUserId).CurrentValue = "";
                    entity.Property(e => e.UpdatedByUserName).CurrentValue = "";
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}