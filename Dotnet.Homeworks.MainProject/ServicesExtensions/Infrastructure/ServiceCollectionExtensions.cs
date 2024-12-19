using Dotnet.Homeworks.Infrastructure.Validation.Behaviors;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;
using FluentValidation;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Features.Helpers.AssemblyReference.Assembly);
            services.AddHttpContextAccessor();
            services.AddPermissionChecks(Features.Helpers.AssemblyReference.Assembly);
            services.AddMediatorPipelines(new[] { Homeworks.Infrastructure.Helpers.AssemblyReference.Assembly },
                typeof(SecurityPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>));

            return services;
        }
    }
}
