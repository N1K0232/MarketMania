namespace MarketMania.StorageProviders;

public interface IStorageProvider
{
    Task SaveAsync(string path, Stream stream, CancellationToken cancellationToken);

    Task<Stream> ReadAsync(string path, CancellationToken cancellationToken);

    async Task<string> ReadAsStringAsync(string path, CancellationToken cancellationToken)
    {
        using var stream = await ReadAsync(path, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(cancellationToken);

        stream.Close();
        return content;
    }

    Task DeleteAsync(string path, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken);
}