namespace MarketMania.Shared.Models.Translations;

public class DetectedLanguageBase
{
    public string Language { get; init; }

    public float Score { get; init; }

    public override string ToString() => Language;
}