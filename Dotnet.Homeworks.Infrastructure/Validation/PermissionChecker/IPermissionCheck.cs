using Dotnet.Homeworks.Infrastructure.Utils;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

//����� PermissionCheck
public interface IPermissionCheck
{
    Task<IEnumerable<PermissionResult>> CheckPermissionAsync<TRequest>(TRequest request);
}

//�������������� PermissionCheck
public interface IPermissionCheck<TRequest>
{
    Task<IEnumerable<PermissionResult>> CheckPermissionAsync(TRequest request);
}