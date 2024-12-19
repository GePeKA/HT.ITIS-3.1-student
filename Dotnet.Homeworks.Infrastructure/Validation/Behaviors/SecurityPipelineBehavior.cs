using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors
{
    public class SecurityPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IPermissionCheck _permissionCheck;

        public SecurityPipelineBehavior(IPermissionCheck permissionCheck)
        {
            _permissionCheck = permissionCheck;
        }

        public async Task<TResponse> Handle(TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var res = await _permissionCheck.CheckPermissionAsync(request);

            if (res.Any(r => r.IsFailure))
            {
                return Result.Create(false, typeof(TResponse), string.Join(" .", res.SelectMany(pr => pr.Error!)));
            }

            return await next();
        }
    }
}
