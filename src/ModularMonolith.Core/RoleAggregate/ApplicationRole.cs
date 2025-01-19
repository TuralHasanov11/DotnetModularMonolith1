using Microsoft.AspNetCore.Identity;
using ModularMonolith.Core.UserAggregate;

namespace ModularMonolith.Core.RoleAggregate;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

    public ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = [];

    private ApplicationRole(string name)
    {
        Name = name;
    }

    public void UpdateName(RoleName name)
    {
        Name = name.Value;
    }

    public static ApplicationRole Create(RoleName name)
    {
        return new ApplicationRole(name.Value);
    }
}