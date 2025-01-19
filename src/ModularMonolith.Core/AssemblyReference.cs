using System.Reflection;

namespace ModularMonolith.Core;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}