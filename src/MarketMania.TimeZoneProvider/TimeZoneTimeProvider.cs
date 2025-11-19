namespace MarketMania.TimeZoneProvider;

public class TimeZoneTimeProvider(ITimeZoneService timeZoneService) : TimeProvider
{
    public override TimeZoneInfo LocalTimeZone => timeZoneService.GetTimeZone() ?? TimeZoneInfo.Utc;
}