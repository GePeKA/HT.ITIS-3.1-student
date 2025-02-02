using System.Reflection;

namespace Dotnet.Homeworks.Infrastructure.Helpers;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
