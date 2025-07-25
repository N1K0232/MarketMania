using System.Text.Json.Serialization;
using MarketMania.Shared.Models.Translations.Enums;

namespace MarketMania.Shared.Models.Translations;

public class ServiceLanguage
{
    public string Code { get; init; }

    public string Name { get; init; }

    public string NativeName { get; init; }

    [JsonPropertyName("dir")]
    public LanguageDirectionality Directionality { get; init; }

    public override string ToString() => Name;
}