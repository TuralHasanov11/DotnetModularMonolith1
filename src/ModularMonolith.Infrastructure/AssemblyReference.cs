using System.Reflection;

namespace ModularMonolith.Infrastructure;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}