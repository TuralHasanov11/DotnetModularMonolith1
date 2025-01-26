using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public required ApplicationUser User { get; set; }
}