namespace ModularMonolith.Users.Core.RoleAggregate;

public static class ApplicationRoles
{
    public const string Administrator = "Administrator";

    public static IEnumerable<RoleName> All()
    {
        yield return RoleName.From(Administrator);
    }
}
