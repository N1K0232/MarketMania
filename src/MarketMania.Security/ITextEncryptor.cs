namespace MarketMania.Security;

public interface ITextEncryptor
{
    Task<string> EncryptAsync(string plaintext, string password, CancellationToken cancellationToken = default);

    Task<string> DecryptAsync(string ciphertext, string password, CancellationToken cancellationToken = default);
}