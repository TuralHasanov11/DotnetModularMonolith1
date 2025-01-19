using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace ModularMonolith.Infrastructure.Data;

public static class ApplicationDbContextExtensions
{
    public static void AddApplicationDbContext(this IServiceCollection services, string connectionString, bool isEnvironmentDevelopment)
    {
        services.AddDbContext<ApplicationDbContext>((_, options) =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptionsAction => npgsqlOptionsAction.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName,
                    ApplicationDbContext.Schema))
                .AddInterceptors(new AuditInterceptor());
            // TODO: AuditInterceptor is not implemented yet

            if (isEnvironmentDevelopment)
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        });
    }
}