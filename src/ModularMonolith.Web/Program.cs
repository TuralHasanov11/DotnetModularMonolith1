using ModularMonolith.Web.Configuration;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseDefaultServiceProvider(config => config.ValidateOnBuild = true);

    builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

    builder.Logging.EnableEnrichment();
    builder.Logging.EnableRedaction();

    builder.Services.Install(builder.Configuration, builder.Environment);

    var app = builder.Build();

    await app.ConfigureAsync();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Error(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program;
