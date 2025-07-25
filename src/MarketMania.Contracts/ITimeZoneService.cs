namespace MarketMania.Contracts;

public interface ITimeZoneService
{
    TimeZoneInfo GetTimeZone();

    string GetTimeZoneHeaderValue();
}