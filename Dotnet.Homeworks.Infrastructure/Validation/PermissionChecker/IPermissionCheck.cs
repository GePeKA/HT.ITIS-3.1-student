using Dotnet.Homeworks.Infrastructure.Utils;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

//Общий PermissionCheck
public interface IPermissionCheck
{
    Task<IEnumerable<PermissionResult>> CheckPermissionAsync<TRequest>(TRequest request);
}

//Типизированный PermissionCheck
public interface IPermissionCheck<TRequest>
{
    Task<IEnumerable<PermissionResult>> CheckPermissionAsync(TRequest request);
}