using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class StorageFactory : IStorageFactory
{
    private readonly IMinioClient _minioClient;

    public StorageFactory(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }   

    public async Task<IStorage<Image>> CreateImageStorageWithinBucketAsync(string bucketName)
    {
        bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));

        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }

        return new ImageStorage(bucketName, _minioClient);
    }
}