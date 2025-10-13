using System.Security.Cryptography;

namespace MarketMania.Security;

public class EncryptionOptions
{
    public int KeySize { get; set; } = 32;

    public int SaltSize { get; set; } = 16;

    public int Iterations { get; set; } = 100_000;

    public string HmacKey { get; set; } = null!;

    public HashAlgorithmName AlgorithmName { get; set; } = HashAlgorithmName.SHA256;
}