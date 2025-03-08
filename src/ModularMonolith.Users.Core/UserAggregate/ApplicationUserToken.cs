using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserToken : IdentityUserToken<IdentityId>
{
    public ApplicationUser User { get; }
}
