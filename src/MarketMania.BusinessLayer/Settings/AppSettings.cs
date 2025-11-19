using Microsoft.AspNetCore.Identity;

namespace MarketMania.BusinessLayer.Settings;

public class AppSettings
{
    public string ApplicationName { get; init; } = "Market Mania";

    public string ApplicationDescription { get; init; } = "A C# web application that represents an online store";

    public int CommandTimeout { get; init; } = 120;

    public TimeSpan DefaultSlidingExpiration { get; init; } = TimeSpan.FromHours(1);

    public bool ExecuteStartup { get; init; } = true;

    public int MaxRetryCount { get; init; } = 10;

    public TimeSpan MaxRetryDelay { get; init; } = TimeSpan.FromSeconds(2);

    public int MaxUploadSize { get; init; } = 20_971_520;

    public string SecurityKey { get; init; } = "";

    public string? StorageFolder { get; init; }

    public string[] SupportedCultures { get; init; } = [];
}