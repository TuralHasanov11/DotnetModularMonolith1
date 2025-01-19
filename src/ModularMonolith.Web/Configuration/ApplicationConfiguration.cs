using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModularMonolith.Core.RoleAggregate;
using ModularMonolith.Core.UserAggregate;
using ModularMonolith.Infrastructure.Data;
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

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.DisplayOperationId());

            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                await SeedData(app, scope);
            }
        }

        app.UseAntiforgery();

        app.UseHttpsRedirection();

        app.UseResponseCaching();
        app.UseResponseCompression();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseRequestLocalization();

        app.UseRequestDecompression();

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();

        app.UseMiddleware<RequestContextLoggingMiddleware>();

        app.UseSerilogRequestLogging();
        app.UseW3CLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseMiddleware<RequestTimeLoggingMiddleware>();
        }

        app.UseHealthChecks("/health");

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
    }

    private static async Task SeedData(WebApplication app, IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        await Infrastructure.Data.SeedData.EnsureSeedDataAsync(
            dbContext,
            userManager,
            roleManager,
            app.Configuration,
            logger);
    }
}