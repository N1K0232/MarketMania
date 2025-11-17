namespace MarketMania.Authentication.DataProtection.Interfaces;

public interface ITimeLimitedDataProtectionService
{
    Task<string> ProtectAsync(string plaintext, TimeSpan lifetime, CancellationToken cancellationToken = default);

    Task<string> UnprotectAsync(string protectedData, CancellationToken cancellationToken = default);
}