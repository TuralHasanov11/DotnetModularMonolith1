using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace ModularMonolith.Users.Infrastructure.Data;

public static class UsersDbContextExtensions
{
    public static void ConfigureUsersDbContext(
        this IServiceCollection services,
        string connectionString,
        bool isDevelopmentEnvironment)
    {
        services.AddDbContext<UsersDbContext>((_, options) =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptionsAction => npgsqlOptionsAction.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName,
                    UsersDbContext.Schema))
                .AddInterceptors(new AuditInterceptor());
            // TODO: AuditInterceptor is not implemented yet

            if (isDevelopmentEnvironment)
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        });
    }
}
