using Microsoft.AspNetCore.Identity;
using ModularMonolith.Users.Core.RoleAggregate;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserRole : IdentityUserRole<IdentityId>
{
    public ApplicationUser User { get; }

    public ApplicationRole Role { get; }
}
