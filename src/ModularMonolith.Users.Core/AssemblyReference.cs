using System.Reflection;

namespace ModularMonolith.Users.Core;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
