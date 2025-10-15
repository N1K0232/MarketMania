namespace MarketMania.BusinessLayer.Settings;

public class AppSettings
{
    public string ApplicationName { get; init; } = "Market Mania";

    public string ApplicationDescription { get; init; } = "A C# web application that represents an online store";

    public bool ExecuteStartup { get; init; } = true;

    public int MaxUploadSize { get; init; }

    public string SecurityKey { get; init; } = "";

    public string StorageFolder { get; init; } = "D:\\MarketMania";

    public string[] SupportedCultures { get; init; } = ["en", "it"];
}