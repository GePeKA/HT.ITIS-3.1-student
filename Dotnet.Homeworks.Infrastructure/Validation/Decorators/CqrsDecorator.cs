using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public class CqrsDecorator<TRequest, TResponse>: ValidationSecurityDecorator<TRequest, TResponse>
    where TRequest : IRequest<TResponse> 
    where TResponse : Result
{
    public CqrsDecorator(IPermissionCheck permissionCheck,
        IEnumerable<IValidator<TRequest>> validators) : base(permissionCheck, validators)
    { }

    public async override Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        => await base.Handle(request, cancellationToken);
}