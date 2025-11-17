using MarketMania.Authentication.DataProtection.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace MarketMania.Authentication.DataProtection;

public class DataProtectionService(IDataProtector protector) : IDataProtectionService
{
    public Task<string> ProtectAsync(string plaintext, CancellationToken cancellationToken = default)
    {
        var protectedData = protector.Protect(plaintext);
        return Task.FromResult(protectedData);
    }

    public Task<string> ProtectAsync(string plaintext, TimeSpan lifetime, CancellationToken cancellationToken = default)
    {
        var protectedData = protector.ToTimeLimitedDataProtector().Protect(plaintext, lifetime);
        return Task.FromResult(protectedData);
    }

    public Task<string> UnprotectAsync(string protectedData, CancellationToken cancellationToken = default)
    {
        var plaintext = protector.Unprotect(protectedData);
        return Task.FromResult(plaintext);
    }
}