using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Users.Core.UserAggregate;

public sealed class ApplicationUser : IdentityUser<IdentityId>
{
    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public FullName FullName => FullName.From(
        UserAggregate.FirstName.From(FirstName),
        UserAggregate.LastName.From(LastName));

    public ICollection<ApplicationUserClaim> Claims { get; } = [];

    public ICollection<ApplicationUserLogin> Logins { get; } = [];

    public ICollection<ApplicationUserToken> Tokens { get; } = [];

    public ICollection<ApplicationUserRole> UserRoles { get; } = [];

    private ApplicationUser(
        string userName,
        string email,
        string firstName,
        string lastName)
    {
        Id = new();
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdateFirstName(FirstName firstName)
    {
        FirstName = firstName.Value;
    }

    public void UpdateLastName(LastName lastName)
    {
        LastName = lastName.Value;
    }

    public static ApplicationUser Create(
        UserName userName,
        Email email,
        FirstName firstName,
        LastName lastName)
    {
        return new ApplicationUser(userName, email, firstName, lastName);
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }
}
