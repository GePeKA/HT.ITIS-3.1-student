using Dotnet.Homeworks.Features.Helpers;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Mediator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMediator(this IServiceCollection services)
    {
        return services.AddMediator(AssemblyReference.Assembly);
    }
}
