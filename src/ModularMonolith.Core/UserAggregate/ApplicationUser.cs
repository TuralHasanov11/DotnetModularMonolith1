using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.Core.UserAggregate;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public FullName FullName => FullName.From(
        UserAggregate.FirstName.From(FirstName),
        UserAggregate.LastName.From(LastName));

    public ICollection<ApplicationUserClaim> Claims { get; set; } = [];

    public ICollection<ApplicationUserLogin> Logins { get; set; } = [];

    public ICollection<ApplicationUserToken> Tokens { get; set; } = [];

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

    private ApplicationUser(
        string userName,
        string email,
        string firstName,
        string lastName)
    {
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
        return new ApplicationUser(userName.Value, email.Value, firstName.Value, lastName.Value);
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }
}