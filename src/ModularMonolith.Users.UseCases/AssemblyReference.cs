using System.Reflection;

namespace ModularMonolith.Users.UseCases;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
