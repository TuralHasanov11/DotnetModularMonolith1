namespace ModularMonolith.Web.Configuration;

public interface IServiceInstaller
{
    void Install(
        IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment);
}