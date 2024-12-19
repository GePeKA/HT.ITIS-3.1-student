using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] handlersAssemblies)
    {
        var requestHandlersAndInterfaces = GetHandlerTypesAndInterfaces(handlersAssemblies);

        foreach (var (handler, iface) in requestHandlersAndInterfaces)
        {
            services.AddScoped(iface, handler);
        }

        return services
            .AddSingleton<IMediator, CustomMediator>();
    }

    public static IServiceCollection AddMediatorPipelines(this IServiceCollection services,
        Assembly[] pipelinesAssemblies,
        Assembly[] handlersAssemblies)
    {
        var requestHandlersAndInterfaces = GetHandlerTypesAndInterfaces(handlersAssemblies);
        var pipelineImplementations = GetPipelineImplementations(pipelinesAssemblies);

        foreach (var (handler, iface) in requestHandlersAndInterfaces)
        {
            if (handler is IHasPipeline)
            {
                foreach (var pipeline in pipelineImplementations)
                {
                    services.AddPipelineBehavior(pipeline, handler);
                }
            }
            
        }

        return services;
    }

    private static void AddPipelineBehavior(this IServiceCollection services, Type pipelineType, Type requestHandlerType)
    {
        var handlerArguments = requestHandlerType.GetGenericArguments();
        var requestType = handlerArguments[0];
        var responseType = handlerArguments[1];
        
        var pipelineInterface = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var pipelineImplementation = pipelineType.MakeGenericType(requestType, responseType);

        services.AddScoped(pipelineInterface, pipelineImplementation);
    }

    private static List<Type> GetPipelineImplementations(params Assembly[] pipelinesAssemblies)
    {
        return pipelinesAssemblies.SelectMany(a => a.GetTypes())
            .Where(t => t
                .GetInterfaces()
                .Any(i => i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)))
            .ToList();
    }

    private static List<(Type handler, Type iface)> GetHandlerTypesAndInterfaces(Assembly[] handlersAssemblies)
    {
        var requestHandlerTypes = handlersAssemblies.SelectMany(a => a.GetTypes())
            .Where(t => t
                .GetInterfaces()
                .Any(IsHandlerInterface))
            .ToList();

        var requestHandlersAndInterfaces = requestHandlerTypes
            .Select(handler => (
                Handler: handler,
                Interface: handler
                    .GetInterfaces()
                    .First(IsHandlerInterface)
            ))
            .ToList();

        return requestHandlersAndInterfaces;
    }

    private static bool IsHandlerInterface(Type i)
    {
        return i.IsGenericType
            && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)
            || i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
    }
}