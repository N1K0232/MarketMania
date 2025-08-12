using System.Net.Http.Headers;
using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMania.Clients.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailClient(this IServiceCollection services, IConfiguration configuration, string sectionName = "EmailSettings")
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.Configure<EmailSettings>(configuration.GetSection(sectionName));
        services.AddSingleton<IEmailClient, EmailClient>();

        return services;
    }

    public static IServiceCollection AddPdfSmithClient(this IServiceCollection services, Action<PdfSmithSettings> optionsAction)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(optionsAction, nameof(optionsAction));

        var settings = new PdfSmithSettings();
        optionsAction.Invoke(settings);

        services.AddHttpClient<IPdfSmithClient, PdfSmithClient>(client =>
        {
            client.BaseAddress = new Uri("https://pdfsmith.azurewebsites.net/");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", settings.SubscriptionKey);
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-time-zone", "Europe/Rome");
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("it-IT"));
        });

        return services;
    }

    public static IServiceCollection AddSentimentApiClient(this IServiceCollection services, Action<SentimentAnalysisSettings> optionsAction)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(optionsAction, nameof(optionsAction));

        var settings = new SentimentAnalysisSettings();
        optionsAction.Invoke(settings);

        services.AddHttpClient<ISentimentAnalysisClient, SentimentAnalysisClient>(client =>
        {
            client.BaseAddress = new Uri("https://twinword-sentiment-analysis.p.rapidapi.com");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-key", settings.SubscriptionKey);
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-host", "twinword-sentiment-analysis.p.rapidapi.com");
        });

        return services;
    }

    public static IServiceCollection AddTranslatorClient(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        services.AddHttpClient<ITranslatorClient, TranslatorClient>(client =>
        {
        });

        return services;
    }
}