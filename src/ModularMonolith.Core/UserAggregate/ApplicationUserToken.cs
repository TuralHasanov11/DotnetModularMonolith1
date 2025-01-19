using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Core.UserAggregate;

public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public required ApplicationUser User { get; set; }
}