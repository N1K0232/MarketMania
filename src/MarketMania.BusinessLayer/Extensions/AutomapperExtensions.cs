using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMania.BusinessLayer.Extensions;

public static class AutomapperExtensions
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        => AddAutoMapper(services, Assembly.GetExecutingAssembly());

    public static IServiceCollection AddAutoMapper(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        services.AddAutoMapper(options =>
        {
            var profiles = new List<Profile>();
            var profileTypes = assembly.GetTypes().Where(t => typeof(Profile).IsAssignableFrom(t));

            foreach (var profileType in profileTypes)
            {
                profiles.Add((Profile)Activator.CreateInstance(profileType)!);
            }

            options.AddProfiles(profiles);
        });

        return services;
    }
}