namespace MarketMania.BusinessLayer.Settings;

public class EmailSettings
{
    public string Host { get; init; }

    public int Port { get; init; }

    public bool UseSsl { get; init; }

    public string SenderName { get; init; }

    public string SenderEmail { get; init; }

    public string UserName { get; init; }

    public string Password { get; init; }

    public bool IgnoreServerCertificateErrors { get; init; }
}