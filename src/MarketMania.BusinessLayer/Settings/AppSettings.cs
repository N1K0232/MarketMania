namespace MarketMania.BusinessLayer.Settings;

public class AppSettings
{
    public string ApplicationName { get; init; }

    public string ApplicationDescription { get; init; }

    public bool ExecuteStartup { get; init; }

    public string[] SupportedCultures { get; init; }
}