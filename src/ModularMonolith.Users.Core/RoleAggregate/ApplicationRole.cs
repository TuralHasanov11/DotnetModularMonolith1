using Microsoft.AspNetCore.Identity;
using ModularMonolith.Users.Core.UserAggregate;

namespace ModularMonolith.Users.Core.RoleAggregate;

public sealed class ApplicationRole : IdentityRole<IdentityId>
{
    public ICollection<ApplicationUserRole> UserRoles { get; } = [];

    public ICollection<ApplicationRoleClaim> RoleClaims { get; } = [];

    private ApplicationRole(string name)
    {
        Id = new();
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
