namespace MarketMania.StorageProviders.FileSystem;

public class FileSystemStorageProvider(FileSystemStorageSettings settings) : IStorageProvider
{
    public async Task SaveAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(settings.StorageFolder, path);
        var directoryName = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        await using var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

        fileStream.Close();
    }

    public Task<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(settings.StorageFolder, path);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        var stream = File.OpenRead(fullPath);
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(settings.StorageFolder, path);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(settings.StorageFolder, path);
        var exists = File.Exists(fullPath);

        return Task.FromResult(exists);
    }
}