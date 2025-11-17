using MarketMania.Contracts;
using TinyHelpers.Extensions;

namespace MarketMania.Services;

public class TimeZoneService(IHttpContextAccessor httpContextAccessor) : ITimeZoneService
{
    public static readonly string HeaderKey = "x-time-zone";

    public TimeZoneInfo? GetTimeZone()
    {
        var timeZoneId = GetTimeZoneHeaderValueCore();
        if (timeZoneId.HasValue() && TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZoneInfo))
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