using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MarketMania.Security;

internal class TextEncryptor(IOptions<EncryptionOptions> optionsAccessor) : ITextEncryptor
{
    private readonly EncryptionOptions options = optionsAccessor.Value;

    public async Task<string> EncryptAsync(string plaintext, string password, CancellationToken cancellationToken = default)
    {
        var salt = RandomNumberGenerator.GetBytes(options.SaltSize);
        using var kdf = new Rfc2898DeriveBytes(password, salt, options.Iterations, options.AlgorithmName);

        var key = kdf.GetBytes(options.KeySize);
        using var aes = Aes.Create();

        aes.Key = key;
        aes.GenerateIV();

        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        await using var memoryStream = new MemoryStream();
        await memoryStream.WriteAsync(salt, cancellationToken);
        await memoryStream.WriteAsync(aes.IV.AsMemory(0, aes.IV.Length), cancellationToken);

        await using var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await using var writer = new StreamWriter(cryptoStream, Encoding.UTF8);

        await writer.WriteAsync(plaintext.AsMemory(), cancellationToken);
        var cipherData = memoryStream.ToArray();

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(options.HmacKey));
        var tag = hmac.ComputeHash(cipherData);

        var finalData = new byte[cipherData.Length + tag.Length];
        Buffer.BlockCopy(cipherData, 0, finalData, 0, cipherData.Length);
        Buffer.BlockCopy(tag, 0, finalData, cipherData.Length, tag.Length);

        return Convert.ToBase64String(finalData);
    }

    public async Task<string> DecryptAsync(string ciphertext, string password, CancellationToken cancellationToken = default)
    {
        var allBytes = Convert.FromBase64String(ciphertext);
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(options.HmacKey));

        var tagSize = hmac.HashSize / 8;
        if (allBytes.Length < tagSize + options.SaltSize + 16)
        {
            throw new ArgumentException("Invalid format", nameof(ciphertext));
        }

        var cipherData = new byte[allBytes.Length - tagSize];
        var tag = new byte[tagSize];

        Buffer.BlockCopy(allBytes, 0, cipherData, 0, cipherData.Length);
        Buffer.BlockCopy(allBytes, cipherData.Length, tag, 0, tagSize);

        var computedTag = hmac.ComputeHash(cipherData);
        if (!CryptographicOperations.FixedTimeEquals(tag, computedTag))
        {
            throw new CryptographicException("Invalid HMAC. Corrupted or manipulated data");
        }

        var pos = 0;
        var salt = new byte[options.SaltSize];

        Buffer.BlockCopy(cipherData, pos, salt, 0, salt.Length);
        pos += salt.Length;

        using var kdf = new Rfc2898DeriveBytes(password, salt, options.Iterations, options.AlgorithmName);
        var key = kdf.GetBytes(options.KeySize);

        using var aes = Aes.Create();
        aes.Key = key;

        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var iv = new byte[aes.BlockSize / 8];
        Buffer.BlockCopy(cipherData, pos, iv, 0, iv.Length);

        aes.IV = iv;
        pos += iv.Length;

        var cipherBytes = new byte[cipherData.Length - pos];
        Buffer.BlockCopy(cipherData, pos, cipherBytes, 0, cipherBytes.Length);

        await using var ms = new MemoryStream(cipherBytes);
        await using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);

        using var reader = new StreamReader(cryptoStream, Encoding.UTF8);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}