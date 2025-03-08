using Microsoft.AspNetCore.Identity;
using ModularMonolith.Users.Core.UserAggregate;

namespace ModularMonolith.Users.Core.RoleAggregate;

public class ApplicationRoleClaim : IdentityRoleClaim<IdentityId>
{
    public ApplicationRole Role { get; }
}
