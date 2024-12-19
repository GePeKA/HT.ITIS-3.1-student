using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator;

public class CustomMediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public CustomMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType)! as dynamic;

        if (handler is IHasPipeline)
        {
            var handlerDelegate = async (dynamic req, CancellationToken ct) => (TResponse) await handler.Handle(req, ct);

            return await BuildPipeline(request, handlerDelegate, cancellationToken);
        }
        else
        {
            return await handler.Handle((dynamic)request, cancellationToken);
        }
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        var handler = _serviceProvider.GetService<IRequestHandler<TRequest>>()!;

        return handler.Handle(request, cancellationToken);
    }

    //Для чего это вообще нужно?
    public Task<dynamic?> Send(dynamic request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        
        return Send(request, cancellationToken);
    }

    //FallBack
    public Task Send<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        return null!;
    }

    private async Task<TResponse> BuildPipeline<TRequest, TResponse>(TRequest request,
        Func<TRequest, CancellationToken, Task<TResponse>> handler,
        CancellationToken cancellationToken = default)
    {
        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(request!.GetType(), typeof(TResponse));

        var pipelineBehaviors = _serviceProvider.GetServices(pipelineType)
           .ToList() as List<dynamic>;

        RequestHandlerDelegate<TResponse> next = () => handler((dynamic) request, cancellationToken);

        foreach (var behavior in pipelineBehaviors)
        {
            var nextClosure = next;
            next = () => behavior.Handle((dynamic) request, nextClosure, cancellationToken);
        }

        return await next();
    }
}
