using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using ModularMonolith.Users.Core.RoleAggregate;
using ModularMonolith.Users.Core.UserAggregate;
using ModularMonolith.Users.Infrastructure.Data;
using Serilog;

namespace ModularMonolith.Web.Configuration;

public static class ApplicationConfiguration
{
    public static async Task ConfigureAsync(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        //app.UseAntiforgery();

        app.UseRouting();
        app.UseRateLimiter();
        app.UseRequestLocalization();
        app.UseCors(Policies.DefaultCorsPolicy);

        app.UseOutputCache();

        app.UseRequestDecompression();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.UseResponseCompression();
        app.UseResponseCaching();
        app.MapStaticAssets();

        app.MapRazorPages();

        app.UseMiddleware<RequestContextLoggingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi()
               .RequireAuthorization(Policies.ApiTesterPolicy)
               .CacheOutput(Policies.OpenApiCachePolicy);

            app.WithSwagger();

            app.UseMiddleware<RequestTimeLoggingMiddleware>();

            await app.InitializeDatabase();
        }


        app.WithHealthChecks();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
    }

    internal static void WithSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DisplayOperationId();
            c.SwaggerEndpoint("/openapi/v1.json", "v1");
        });
    }

    internal static async Task InitializeDatabase(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        await InitializeUsersDatabase(app, scope);
    }

    private static async Task InitializeUsersDatabase(WebApplication app, AsyncServiceScope scope)
    {
        await using var usersDbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

        await Users.Infrastructure.Data.SeedData.EnsureSeedDataAsync(
            usersDbContext,
            userManager,
            roleManager,
            app.Configuration,
            logger);
    }

    internal static IApplicationBuilder WithHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
        });

        return app;
    }
}


