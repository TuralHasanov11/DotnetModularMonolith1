using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace ModularMonolith.Users.Infrastructure.Data;

public static class UsersDbContextExtensions
{
    public static void ConfigureUsersDbContext(
        this IServiceCollection services,
        string connectionString,
        bool isDevelopmentEnvironment)
    {
        services.AddKeyedScoped<List<AuditEntry>>("Audit", (_, _) => []);

        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptionsAction => npgsqlOptionsAction.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName,
                    UsersDbContext.Schema))
                .AddInterceptors(
                    new AuditInterceptor(
                        sp.GetRequiredKeyedService<List<AuditEntry>>("Audit"),
                        sp.GetRequiredService<IPublishEndpoint>()));

            if (isDevelopmentEnvironment)
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        });

    }
}
