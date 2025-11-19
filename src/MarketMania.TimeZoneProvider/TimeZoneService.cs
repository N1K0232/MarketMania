using Microsoft.AspNetCore.Http;

namespace MarketMania.TimeZoneProvider;

public class TimeZoneService(IHttpContextAccessor httpContextAccessor) : ITimeZoneService
{
    public static readonly string HeaderKey = "x-time-zone";

    public TimeZoneInfo? GetTimeZone()
    {
        var timeZoneId = GetTimeZoneHeaderValueCore();
        if (!string.IsNullOrWhiteSpace(timeZoneId) && TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZoneInfo))
        {
            return timeZoneInfo;
        }

        return null;
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