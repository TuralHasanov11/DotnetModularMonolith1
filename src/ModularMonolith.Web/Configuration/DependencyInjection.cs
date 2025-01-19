using System.Reflection;

namespace ModularMonolith.Web.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection InstallServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        params Assembly[] assemblies)
    {
        IEnumerable<IServiceInstaller> serviceInstallers = assemblies
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(typeInfo => typeof(IServiceInstaller).IsAssignableFrom(typeInfo)
                && !typeInfo.IsInterface
                && !typeInfo.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (IServiceInstaller serviceInstaller in serviceInstallers)
        {
            serviceInstaller.Install(services, configuration, environment);
        }

        return services;
    }
}