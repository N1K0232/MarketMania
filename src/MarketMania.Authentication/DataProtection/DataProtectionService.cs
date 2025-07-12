using Microsoft.AspNetCore.DataProtection;

namespace MarketMania.Authentication.DataProtection;

public class DataProtectionService(ITimeLimitedDataProtector protector) : IDataProtectionService
{
    public Task<string> ProtectAsync(string plaintext, TimeSpan lifetime, CancellationToken cancellationToken = default)
    {
        var protectedData = protector.Protect(plaintext, lifetime);
        return Task.FromResult(protectedData);
    }

    public Task<string> UnprotectAsync(string protectedData, CancellationToken cancellationToken = default)
    {
        var plaintext = protector.Unprotect(protectedData);
        return Task.FromResult(plaintext);
    }
}