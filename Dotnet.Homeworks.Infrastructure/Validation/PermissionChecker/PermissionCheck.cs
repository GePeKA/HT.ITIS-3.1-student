using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

public class PermissionCheck : IPermissionCheck
{
    IServiceProvider _serviceProvider;

    public PermissionCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<PermissionResult>> CheckPermissionAsync<TRequest>(TRequest request)
    {
        if (!typeof(TRequest)
            .GetInterfaces()
            .Any(x =>
                x == typeof(IClientRequest) 
                || x == typeof(IAdminRequest)))
        {
            return new List<PermissionResult> { new(true) };
        }

        var permissionCheckType = request is IAdminRequest
            ? typeof(IPermissionCheck<>).MakeGenericType(typeof(IAdminRequest))
            : typeof(IPermissionCheck<>).MakeGenericType(typeof(IClientRequest));

        var permissionCheck = _serviceProvider.GetService(permissionCheckType) as dynamic;
        return await permissionCheck!.CheckPermissionAsync(request);
    }
}
