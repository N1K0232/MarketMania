namespace MarketMania.BusinessLayer.Settings;

public class EmailSettings
{
    public string Host { get; set; }

    public int Port { get; set; }

    public bool UseSsl { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public bool IgnoreServerCertificateErrors { get; set; }
}