namespace MarketMania.BusinessLayer.Resources;

internal static class Constants
{
    public const string GlobalAuthorizationUrl = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
    public const string RegionAuthorizationUrl = "https://{0}.api.cognitive.microsoft.com/sts/v1.0/issueToken";

    public const string AuthorizationHeader = "Authorization";

    public const string JsonMediaType = "application/json";
    public const string WavAudioMediaType = "audio/wav";

    public const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
    public const string OcpApimSubscriptionRegionHeader = "Ocp-Apim-Subscription-Region";

    public const int MaxArrayLengthForTranslation = 25;
    public const int MaxTextLengthForTranslation = 5000;
    public const int MaxArrayLengthForDetection = 100;
    public const int MaxTextLengthForDetection = 10000;
}