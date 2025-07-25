namespace MarketMania.Shared.Models.Translations;

public class DetectedLanguage : DetectedLanguageBase
{
    public bool IsTranslationSupported { get; init; }

    public bool IsTransliterationSupported { get; init; }
}