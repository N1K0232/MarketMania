namespace MarketMania.Shared.Models.Translations;

public class DetectedLanguageResponse : DetectedLanguage
{
    public IEnumerable<DetectedLanguage> Alternatives { get; init; } = [];
}