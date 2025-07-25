using System.Runtime.Serialization;

namespace MarketMania.Shared.Models.Translations.Enums;

public enum LanguageDirectionality
{
    [EnumMember(Value = "ltr")]
    LeftToRight,

    [EnumMember(Value = "rtl")]
    RightToLeft
}