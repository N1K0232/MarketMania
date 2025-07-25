namespace MarketMania.Shared.Models.Translations;

public class TranslationResponse
{
    public DetectedLanguageBase DetectedLanguage { get; init; }

    public IEnumerable<Translation> Translations { get; init; } = [];

    public Translation Translation => Translations.FirstOrDefault();
}