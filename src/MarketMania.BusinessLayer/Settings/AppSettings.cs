namespace MarketMania.BusinessLayer.Settings;

public class AppSettings
{
    public string ApplicationName { get; init; }

    public string ApplicationDescription { get; init; }

    public bool ExecuteStartup { get; init; }

    public int MaxUploadSize { get; init; }

    public string PdfSmithSubscriptionKey { get; init; }

    public string SentimentSubscriptionKey { get; init; }

    public string StorageFolder { get; init; }

    public string[] SupportedCultures { get; init; }
}