using Dotnet.Homeworks.Storage.API.Configuration;
using Minio;

namespace Dotnet.Homeworks.Storage.API.ServicesExtensions;

public static class AddMinioExtensions
{
    public static IServiceCollection AddMinioClient(this IServiceCollection services,
        MinioConfig minioConfiguration)
    {
        services.AddSingleton<IMinioClient, MinioClient>(_ =>
        {
            return new MinioClient()
                .WithCredentials(minioConfiguration.Username, minioConfiguration.Password)
                .WithEndpoint(minioConfiguration.Endpoint, minioConfiguration.Port)
                .WithSSL(minioConfiguration.WithSsl)
                .Build();
        });

        return services;
    }
}