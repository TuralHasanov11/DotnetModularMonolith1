using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Users.Core.UserAggregate;

public class ApplicationUserLogin : IdentityUserLogin<Guid>
{
    public required ApplicationUser User { get; set; }
}