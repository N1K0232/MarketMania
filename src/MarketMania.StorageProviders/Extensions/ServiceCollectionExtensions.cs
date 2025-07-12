using MarketMania.StorageProviders.AzureStorage;
using MarketMania.StorageProviders.FileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMania.StorageProviders.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, Action<AzureStorageSettings> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(setupAction, nameof(setupAction));

        var settings = new AzureStorageSettings();
        setupAction.Invoke(settings);

        services.AddSingleton(settings);
        services.AddScoped<IStorageProvider, AzureStorageProvider>();

        return services;
    }

    public static IServiceCollection AddFileSystemStorage(this IServiceCollection services, Action<FileSystemStorageSettings> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(setupAction, nameof(setupAction));

        var settings = new FileSystemStorageSettings();
        setupAction.Invoke(settings);

        services.AddSingleton(settings);
        services.AddScoped<IStorageProvider, FileSystemStorageProvider>();

        return services;
    }
}