namespace Dotnet.Homeworks.Mediator;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

//Интерфейс для того, чтобы пометить у каких handler нужно вызывать пайплайн
public interface IHasPipeline { }