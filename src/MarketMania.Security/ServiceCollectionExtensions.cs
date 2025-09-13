using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMania.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEncryption(this IServiceCollection services, IConfiguration configuration, string sectionName = "Encryption")
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.Configure<EncryptionOptions>(configuration.GetSection(sectionName));
        services.AddSingleton<ITextEncryptor, TextEncryptor>();

        return services;
    }
}