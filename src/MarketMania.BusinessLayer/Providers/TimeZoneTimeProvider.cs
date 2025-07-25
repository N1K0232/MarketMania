using MarketMania.Contracts;

namespace MarketMania.BusinessLayer.Providers;

public class TimeZoneTimeProvider(ITimeZoneService timeZoneService) : TimeProvider
{
    public override TimeZoneInfo LocalTimeZone => timeZoneService.GetTimeZone() ?? TimeZoneInfo.Utc;
}