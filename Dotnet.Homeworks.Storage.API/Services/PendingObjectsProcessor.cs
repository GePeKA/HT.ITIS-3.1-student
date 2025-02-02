using Dotnet.Homeworks.Storage.API.Constants;

namespace Dotnet.Homeworks.Storage.API.Services;

public class PendingObjectsProcessor : BackgroundService
{
    private readonly IStorageFactory _storageFactory;

    public PendingObjectsProcessor(IStorageFactory storageFactory)
    {
        _storageFactory = storageFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pendingStorage = await _storageFactory.CreateImageStorageWithinBucketAsync(Buckets.Pending);

        while (!stoppingToken.IsCancellationRequested)
        {
            var pendingItemsNames = await pendingStorage.EnumerateItemNamesAsync(stoppingToken);

            foreach (var itemName in pendingItemsNames)
            {
                var item = await pendingStorage.GetItemAsync(itemName, stoppingToken);

                if (item!.Metadata.TryGetValue(MetadataKeys.Destination, out var destinationBucket))
                {
                    await pendingStorage.CopyItemToBucketAsync(itemName, destinationBucket, stoppingToken);
                }

                await pendingStorage.RemoveItemAsync(itemName, stoppingToken);
            }

            await Task.Delay(PendingObjectProcessor.Period);
        }
    }
}