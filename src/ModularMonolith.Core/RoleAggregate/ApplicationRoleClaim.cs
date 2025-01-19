using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Core.RoleAggregate;

public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public required ApplicationRole Role { get; set; }
}