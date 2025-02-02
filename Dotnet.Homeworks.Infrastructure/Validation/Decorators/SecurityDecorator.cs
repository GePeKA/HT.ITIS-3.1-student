using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public class SecurityDecorator<TRequest, TResponse>: IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IPermissionCheck _permissionCheck;

    public SecurityDecorator(IPermissionCheck permissionCheck)
    {
        _permissionCheck = permissionCheck;
    }

    public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var permissionResult = await _permissionCheck.CheckPermissionAsync(request);

        if (permissionResult.Any(pr => pr.IsFailure))
        {
            return Result.Create(
                false, 
                typeof(TResponse),
                error: string.Join(". ",
                    permissionResult
                        .Where(pr => pr.IsFailure)
                        .Select(pr => pr.Error)
                )
            );
        }
        else
        {
            return Result.Create(true, typeof(TResponse));
        }
    }
}
