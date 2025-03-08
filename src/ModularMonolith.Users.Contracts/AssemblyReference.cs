using System.Reflection;

namespace ModularMonolith.Users.Contracts;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
