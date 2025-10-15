using MarketMania.Contracts;

namespace MarketMania.Services;

public class TimeZoneService(IHttpContextAccessor httpContextAccessor) : ITimeZoneService
{
    public static readonly string HeaderKey = "x-time-zone";

    public TimeZoneInfo? GetTimeZone()
    {
        var timeZoneId = GetTimeZoneHeaderValueCore();
        if (timeZoneId is null || !TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZoneInfo))
        {
            return null;
        }

        return timeZoneInfo;
    }

    public string? GetTimeZoneHeaderValue()
        => GetTimeZoneHeaderValueCore();

    private string? GetTimeZoneHeaderValueCore()
    {
        if (httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(HeaderKey, out var timeZone) ?? false)
        {
            return timeZone.ToString();
        }

        return null;
    }
}