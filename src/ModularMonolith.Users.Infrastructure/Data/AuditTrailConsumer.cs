using MassTransit;
using SharedKernel;

namespace ModularMonolith.Users.Infrastructure.Data;

public class AuditTrailConsumer(UsersDbContext dbContext) : IConsumer<AuditTrailMessage>
{
    private readonly UsersDbContext _dbContext = dbContext;

    public async Task Consume(ConsumeContext<AuditTrailMessage> context)
    {
        _dbContext.AuditEntries.AddRange(context.Message.Entries);
        await _dbContext.SaveChangesAsync();
    }
}
