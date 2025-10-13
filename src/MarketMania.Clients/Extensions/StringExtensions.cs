namespace MarketMania.Clients.Extensions;

internal static class StringExtensions
{
    internal static bool HasValue(this string? input)
        => HasValue(input, false, true);

    private static bool HasValue(string? input, bool allowEmptyString, bool whiteSpaceAsEmpty)
        => allowEmptyString ? input is not null : whiteSpaceAsEmpty ? !string.IsNullOrWhiteSpace(input) : !string.IsNullOrEmpty(input);
}