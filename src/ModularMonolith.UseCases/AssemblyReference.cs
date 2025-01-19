using System.Reflection;

namespace ModularMonolith.UseCases;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}