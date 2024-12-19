using System.Reflection;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{   
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        params Assembly[] permissionChecksAssemblies
    )
    {
        var permissionCheckTypesAndInterfaces = GetPermissionCheckTypesAndInterfaces(permissionChecksAssemblies);
        foreach (var (permissionCheck, iface) in permissionCheckTypesAndInterfaces)
        {
            serviceCollection.AddScoped(iface, permissionCheck);
        }

        serviceCollection.AddScoped<IPermissionCheck, PermissionCheck>();
    }

    private static List<(Type handler, Type iface)> GetPermissionCheckTypesAndInterfaces(Assembly[] permissionChecksAssemblies)
    {
        var requestHandlerTypes = permissionChecksAssemblies.SelectMany(a => a.GetTypes())
            .Where(t => t
                .GetInterfaces()
                .Any(IsPermissionCheckInterface))
            .ToList();

        var requestHandlersAndInterfaces = requestHandlerTypes
            .Select(handler => (
                Handler: handler,
                Interface: handler
                    .GetInterfaces()
                    .First(IsPermissionCheckInterface)
            ))
            .ToList();

        return requestHandlersAndInterfaces;
    }

    private static bool IsPermissionCheckInterface(Type i)
    {
        return i.IsGenericType
            && (i.GetGenericTypeDefinition() == typeof(IPermissionCheck<>));
    }
}