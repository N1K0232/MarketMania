using System;
using System.Collections.Generic;
using System.Text;
using MarketMania.Authentication.DataProtection.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace MarketMania.Authentication.DataProtection;

public class TimeLimitedDataProtectionService(ITimeLimitedDataProtector protector) : ITimeLimitedDataProtectionService
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