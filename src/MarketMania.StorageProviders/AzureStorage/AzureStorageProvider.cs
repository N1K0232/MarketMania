using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MimeMapping;

namespace MarketMania.StorageProviders.AzureStorage;

public class AzureStorageProvider(AzureStorageSettings settings) : IStorageProvider
{
    private readonly BlobServiceClient blobServiceClient = new BlobServiceClient(settings.ConnectionString);

    public async Task SaveAsync(string path, Stream stream, CancellationToken cancellationToken)
    {
        var blobClient = await GetBlobClientAsync(path, true, cancellationToken);
        var headers = new BlobHttpHeaders
        {
            ContentType = MimeUtility.GetMimeMapping(path)
        };

        stream.Position = 0;
        await blobClient.UploadAsync(stream, headers, cancellationToken: cancellationToken);
    }

    public async Task<Stream> ReadAsync(string path, CancellationToken cancellationToken)
    {
        var blobClient = await GetBlobClientAsync(path, cancellationToken: cancellationToken);
        var blobExists = await blobClient.ExistsAsync(cancellationToken);

        if (!blobExists)
        {
            return null;
        }

        var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        return stream;
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.ContainerName);
        await blobContainerClient.DeleteBlobIfExistsAsync(path, cancellationToken: cancellationToken);
    }

    private async Task<BlobClient> GetBlobClientAsync(string path, bool createIfNotExists = false, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.ContainerName);

        if (createIfNotExists)
        {
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        }

        return blobContainerClient.GetBlobClient(path);
    }
}