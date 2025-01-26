using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Users.Infrastructure.Data;

namespace ModularMonolith.Users.Web;

public static class DependencyInjection
{
    public static void InstallUsersModuleServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopmentEnvironment)
    {
        ConfigureDbContext(services, configuration, isDevelopmentEnvironment);
        ConfigureCqrs(services);
        ConfigureHealthChecks(services);



    }

    private static void ConfigureDbContext(
        IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopmentEnvironment)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' not found.");

        services.ConfigureUsersDbContext(connectionString, isDevelopmentEnvironment);
    }

    private static void ConfigureCqrs(IServiceCollection services)
    {
        services.AddMediatR(config
            => config.RegisterServicesFromAssembly(UseCases.AssemblyReference.Assembly));
    }

    private static void ConfigureHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<UsersDbContext>();
    }
}
