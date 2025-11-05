using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MimeMapping;

namespace MarketMania.StorageProviders.AzureStorage;

public class AzureStorageProvider(AzureStorageSettings settings) : IStorageProvider
{
    private readonly BlobServiceClient blobServiceClient = new BlobServiceClient(settings.ConnectionString);

    public async Task SaveAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        var blobClient = await GetBlobClientAsync(path, true, cancellationToken).ConfigureAwait(false);
        var headers = new BlobHttpHeaders
        {
            ContentType = MimeUtility.GetMimeMapping(path)
        };

        stream.Position = 0;
        await blobClient.UploadAsync(stream, headers, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default)
    {
        var blobClient = await GetBlobClientAsync(path, cancellationToken: cancellationToken).ConfigureAwait(false);
        var blobExists = await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);

        if (!blobExists)
        {
            return null;
        }

        var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken = default).ConfigureAwait(false);
        return stream;
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.ContainerName);
        await blobContainerClient.DeleteBlobIfExistsAsync(path, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var blobClient = await GetBlobClientAsync(path, cancellationToken: cancellationToken).ConfigureAwait(false);
        var exists = await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);

        return exists;
    }

    private async Task<BlobClient> GetBlobClientAsync(string path, bool createIfNotExists = false, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.ContainerName);

        if (createIfNotExists)
        {
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return blobContainerClient.GetBlobClient(path);
    }
}