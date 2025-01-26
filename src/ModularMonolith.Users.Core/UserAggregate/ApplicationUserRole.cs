using Microsoft.AspNetCore.Identity;
using ModularMonolith.Users.Core.RoleAggregate;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public required ApplicationUser User { get; set; }
    public required ApplicationRole Role { get; set; }
}