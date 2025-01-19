using Microsoft.AspNetCore.Identity;
using ModularMonolith.Core.RoleAggregate;

namespace ModularMonolith.Core.UserAggregate;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public required ApplicationUser User { get; set; }
    public required ApplicationRole Role { get; set; }
}