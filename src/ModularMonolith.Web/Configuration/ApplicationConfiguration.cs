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
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAntiforgery();

        app.UseRouting();
        app.UseRateLimiter();
        app.UseRequestLocalization();
        app.UseCors(Policies.DefaultCorsPolicy);

        app.UseOutputCache();

        app.MapOpenApi()
                //.RequireAuthorization(Policies.ApiTesterPolicy)
                .CacheOutput(Policies.OpenApiCachePolicy);

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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DisplayOperationId();
                c.SwaggerEndpoint("/openapi/v1.json", "v1");
            });

            app.UseMiddleware<RequestTimeLoggingMiddleware>();

            using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            await SeedData(app, scope);
        }


        app.MapHealthChecks("/health");
        app.MapHealthChecks("/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
        });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
    }

    private static async Task SeedData(WebApplication app, IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        await Users.Infrastructure.Data.SeedData.EnsureSeedDataAsync(
            dbContext,
            userManager,
            roleManager,
            app.Configuration,
            logger);
    }
}
