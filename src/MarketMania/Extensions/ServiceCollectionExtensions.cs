using Microsoft.AspNetCore.Http.Features;

namespace MarketMania.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureFormOptions(this IServiceCollection services, Action<FormOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));

        return services.Configure(configureOptions);
    }
}