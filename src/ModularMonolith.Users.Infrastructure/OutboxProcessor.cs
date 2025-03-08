using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ModularMonolith.Users.Core.Outbox;
using ModularMonolith.Users.Infrastructure.Data;

namespace ModularMonolith.Users.Infrastructure;

public class OutboxProcessor(
    IPublishEndpoint publishEndpoint,
    UsersDbContext dbContext) : IOutboxProcessor
{
    private const int BatchSize = 10;

    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    private readonly UsersDbContext _dbContext = dbContext;


    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {

        var outboxMessages = await _dbContext
            .OutboxMessages.Where(m => m.ProcessedOnUtc == null)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in outboxMessages)
        {
            try
            {
                Type? messageType = Contracts.AssemblyReference.Assembly.GetType(message.Type);

                if (messageType == null)
                {
                    // Log the error
                    continue;
                }

                var deserializedMessage = JsonSerializer.Deserialize(message.Content, messageType)!;

                await _publishEndpoint.Publish(deserializedMessage, messageType, cancellationToken);

                message.Process(DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                message.Fail(ex.ToString(), DateTime.UtcNow);
            }
        }


        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
