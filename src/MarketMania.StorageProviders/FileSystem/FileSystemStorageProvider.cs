namespace MarketMania.StorageProviders.FileSystem;

public class FileSystemStorageProvider(FileSystemStorageSettings settings) : IStorageProvider
{
    public async Task SaveAsync(string path, Stream stream, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(settings.StorageFolder, path);
        var directoryName = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        using var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken);

        fileStream.Close();
    }

    public Task<Stream> ReadAsync(string path, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(settings.StorageFolder, path);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream>(null);
        }

        var stream = File.OpenRead(fullPath);
        return Task.FromResult<Stream>(stream);
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(settings.StorageFolder, path);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}