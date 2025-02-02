using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Storage.API.Constants;
using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;
using System.Reactive.Linq;

namespace Dotnet.Homeworks.Storage.API.Services;

public class ImageStorage : IStorage<Image>
{
    private readonly string _bucket;
    private readonly IMinioClient _minioClient;

    public ImageStorage(string bucketName, IMinioClient minioClient)
    {
        _bucket = bucketName;
        _minioClient = minioClient;
    }

    public async Task<Result> PutItemAsync(Image item, CancellationToken cancellationToken = default)
    {
        if (!item.Metadata.ContainsKey(MetadataKeys.Destination))
        {
            item.Metadata.Add(MetadataKeys.Destination, _bucket);
        }
        else if (item.Metadata[MetadataKeys.Destination] != _bucket)
        {
            return new Result(false, $"Cannot put item to bucket: this Storage's bucket is {_bucket}");
        }

        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_bucket)
                .WithObject(item.FileName);
            await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);

            //Если ошибка не была выброшена, значит объект существует в бакете
            return new Result(false, "Object with such name already exists in bucket!");
        }
        catch { }

        try
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(Buckets.Pending)
                .WithObject(item.FileName)
                .WithContentType(item.ContentType)
                .WithObjectSize(item.Content.Length)
                .WithHeaders(item.Metadata)
                .WithStreamData(item.Content);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }

    public async Task<Image?> GetItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var itemContent = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(itemName)
                .WithCallbackStream(async (stream) =>
                {
                    await stream.CopyToAsync(itemContent);
                });
            var objectStat = await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
            var item = new Image(itemContent, objectStat.ObjectName, objectStat.ContentType, objectStat.MetaData);

            return item;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Result> RemoveItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(itemName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }

    public async Task<IEnumerable<string>> EnumerateItemNamesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var listObjectsArg = new ListObjectsArgs()
            .WithBucket(_bucket);

            var itemNames = await _minioClient.ListObjectsAsync(listObjectsArg, cancellationToken)
                .Select(item => item.Key)
                .ToList();

            return itemNames;
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }

    public async Task<Result> CopyItemToBucketAsync(string itemName, string destinationBucketName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(destinationBucketName);
            if (!await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken))
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(destinationBucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs);
            }

            var copyObjectArgs = new CopyObjectArgs()
                .WithCopyObjectSource(
                    new CopySourceObjectArgs()
                        .WithBucket(_bucket)
                        .WithObject(itemName)
                ).WithBucket(destinationBucketName);

            await _minioClient.CopyObjectAsync(copyObjectArgs, cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }
}