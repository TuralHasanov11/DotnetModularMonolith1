using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel;

namespace ModularMonolith.Users.Infrastructure.Data;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var entries = eventData.Context.ChangeTracker
                .Entries<ISoftDeletable>()
                .Where(e => e.State is EntityState.Deleted);

            foreach (var entity in entries)
            {
                entity.State = EntityState.Modified;
                entity.Property(e => e.IsDeleted).CurrentValue = true;
                entity.Property(e => e.DeletedOnUtc).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
