using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserLogin : IdentityUserLogin<IdentityId>
{
    public ApplicationUser User { get; }
}
