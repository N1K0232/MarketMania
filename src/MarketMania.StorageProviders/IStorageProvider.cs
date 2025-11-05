namespace MarketMania.StorageProviders;

public interface IStorageProvider
{
    Task SaveAsync(string path, Stream stream, CancellationToken cancellationToken = default);

    async Task SaveAsync(string path, byte[] content, CancellationToken cancellationToken = default)
    {
        await using var memoryStream = new MemoryStream(content);
        await SaveAsync(path, memoryStream, cancellationToken).ConfigureAwait(false);
    }

    Task<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default);

    async Task<byte[]?> ReadAsByteArrayAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = await ReadAsync(path, cancellationToken).ConfigureAwait(false);
        if (stream is null)
        {
            return null;
        }

        await using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);

        stream.Close();
        return memoryStream.ToArray();
    }

    async Task<string?> ReadAsStringAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = await ReadAsync(path, cancellationToken).ConfigureAwait(false);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

        stream.Close();
        return content;
    }

    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
}