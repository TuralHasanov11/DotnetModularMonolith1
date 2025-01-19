namespace ModularMonolith.Core.RoleAggregate;

public static class ApplicationRoles
{
    public const string Administrator = "Administrator";

    public const string Student = "Student";

    public const string Instructor = "Instructor";

    public const string Editor = "Editor";

    public static IEnumerable<RoleName> All()
    {
        yield return RoleName.From(Administrator);
        yield return RoleName.From(Student);
        yield return RoleName.From(Instructor);
        yield return RoleName.From(Editor);
    }
}