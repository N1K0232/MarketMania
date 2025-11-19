using MarketMania.TimeZoneProvider;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMania.TimeZoneProvider;

public static class TimeZoneProviderServiceCollectionExtensions
{
    public static IServiceCollection AddTimeZoneProvider(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<TimeZoneTimeProvider>();
        services.AddSingleton<ITimeZoneService, TimeZoneService>();

        return services;
    }
}