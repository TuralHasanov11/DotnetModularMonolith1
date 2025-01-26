using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularMonolith.Users.Core.RoleAggregate;
using ModularMonolith.Users.Core.UserAggregate;

namespace ModularMonolith.Users.Infrastructure.Data;

public static class SeedData
{
    public static async ValueTask EnsureSeedDataAsync(
        UsersDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ILogger logger)
    {
        await dbContext.Database.MigrateAsync();

        await AddRoles(roleManager, logger);
        await AddSuperUser(userManager, configuration, logger);

        await dbContext.SaveChangesAsync();
    }

    private static async Task AddSuperUser(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var userName = UserName.From(configuration["Admin:UserName"]!);

        var admin = await userManager.FindByNameAsync(userName);
        if (admin is null)
        {
            var email = Email.From(configuration["Admin:Email"]!);
            var firstName = FirstName.From(configuration["Admin:FirstName"]!);
            var lastName = LastName.From(configuration["Admin:LastName"]!);
            var password = configuration["Admin:Password"];

            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            admin = ApplicationUser.Create(userName, email, firstName, lastName);

            admin.ConfirmEmail();

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.First().Description);
            }

            await userManager.AddToRoleAsync(admin, ApplicationRoles.Administrator);
            await userManager.AddToRoleAsync(admin, ApplicationRoles.Instructor);
            await userManager.AddToRoleAsync(admin, ApplicationRoles.Student);
            await userManager.AddToRoleAsync(admin, ApplicationRoles.Editor);

            logger.LogDebug("Admin with Name = {UserName} created", userName);
        }
        else
        {
            logger.LogDebug("Admin with Name = {UserName} created", userName);
        }
    }

    private static async ValueTask AddRoles(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        if (!await roleManager.Roles.AnyAsync())
        {
            await AddRole(roleManager, ApplicationRoles.Administrator, logger);
            await AddRole(roleManager, ApplicationRoles.Student, logger);
            await AddRole(roleManager, ApplicationRoles.Instructor, logger);
            await AddRole(roleManager, ApplicationRoles.Editor, logger);
        }
    }

    private static async ValueTask AddRole(
        RoleManager<ApplicationRole> roleManager,
        string roleName,
        ILogger logger)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            try
            {
                var role = ApplicationRole.Create(RoleName.From(roleName));
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.First().Description);
                }

                logger.LogDebug("Role with Name = {Role} created", roleName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred creating Role with Name = {Role} created", roleName);
            }
        }
        else
        {
            logger.LogDebug("Role with Name = {Role} already exists", roleName);
        }
    }
}
