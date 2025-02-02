using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public class ValidationSecurityDecorator<TRequest, TResponse> : SecurityDecorator<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationSecurityDecorator(IPermissionCheck permissionCheck,
        IEnumerable<IValidator<TRequest>> validators) : base(permissionCheck)
    {
        _validators = validators;
    }

    public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var securityCheck = await base.Handle(request, cancellationToken);
        if (securityCheck.IsFailure)
        {
            return securityCheck;
        }

        if (_validators.Count() == 0)
        {
            return Result.Create(true, typeof(TResponse));
        }

        var validationResultsTasks = _validators.Select(async x => 
            await x.ValidateAsync(request, cancellationToken)).ToList();
        var validationResults = await Task.WhenAll(validationResultsTasks);

        if (validationResults.Any(vr => !vr.IsValid))
        {
            var failureResults = validationResults.SelectMany(x => x.Errors);
            var errors = failureResults.Select(fr => fr.ErrorMessage);

            return Result.Create(false, typeof(TResponse), string.Join(". ", errors));
        }
        else
        {
            return Result.Create(true, typeof(TResponse));
        }
    }
}
