using Dotnet.Homeworks.DataAccess.Repositories;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.DataAccess
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
        {
            return services
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
