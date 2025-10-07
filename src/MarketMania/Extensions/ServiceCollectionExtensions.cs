using MarketMania.BusinessLayer.Providers;
using MarketMania.Contracts;
using MarketMania.Services;
using Microsoft.AspNetCore.Http.Features;

namespace MarketMania.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTimeZoneProvider(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<TimeZoneTimeProvider>();
        services.AddSingleton<ITimeZoneService, TimeZoneService>();

        return services;
    }

    public static IServiceCollection ConfigureFormOptions(this IServiceCollection services, Action<FormOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));

        return services.Configure(configureOptions);
    }
}