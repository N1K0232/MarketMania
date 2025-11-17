namespace MarketMania.Authentication.DataProtection.Interfaces;

public interface IDataProtectionService
{
    Task<string> ProtectAsync(string plaintext, CancellationToken cancellationToken = default);

    Task<string> ProtectAsync(string plaintext, TimeSpan lifetime, CancellationToken cancellationToken = default);

    Task<string> UnprotectAsync(string protectedData, CancellationToken cancellationToken = default);
}