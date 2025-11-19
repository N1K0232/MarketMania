namespace MarketMania.TimeZoneProvider;

public interface ITimeZoneService
{
    TimeZoneInfo? GetTimeZone();

    string? GetTimeZoneHeaderValue();
}