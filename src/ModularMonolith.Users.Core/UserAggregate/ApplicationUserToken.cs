using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public required ApplicationUser User { get; set; }
}