using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserClaim : IdentityUserClaim<IdentityId>
{
    public ApplicationUser User { get; }
}
