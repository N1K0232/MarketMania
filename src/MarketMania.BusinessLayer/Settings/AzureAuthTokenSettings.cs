namespace MarketMania.BusinessLayer.Settings;

public class AzureAuthTokenSettings
{
    public TimeSpan TokenDuration { get; init; } = TimeSpan.FromMinutes(10);
}