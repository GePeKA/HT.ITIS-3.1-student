using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
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
                return await next();
            }
        }
    }
}
