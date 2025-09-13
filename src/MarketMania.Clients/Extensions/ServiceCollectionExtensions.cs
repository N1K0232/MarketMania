using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace MarketMania.Clients.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailClient(this IServiceCollection services, IConfiguration configuration, string sectionName = "EmailSettings")
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.Configure<EmailSettings>(configuration.GetSection(sectionName));
        services.AddScoped<IEmailClient, EmailClient>();

        return services;
    }

    public static IServiceCollection AddPdfSmith(this IServiceCollection services, IConfiguration configuration, string sectionName = "PdfSmith")
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var section = configuration.GetSection(sectionName);

        services.AddHttpClient<IPdfSmithClient, PdfSmithClient>(client =>
        {
            client.BaseAddress = new Uri("https://pdfsmith.azurewebsites.net/");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", section["SubscriptionKey"]);
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-time-zone", "Europe/Rome");
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("it-IT"));
        });

        return services;
    }

    public static IServiceCollection AddSentimentApi(this IServiceCollection services, IConfiguration configuration, string sectionName = "SentimentAnalysis")
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var section = configuration.GetSection(sectionName);

        services.AddHttpClient<ISentimentAnalysisClient, SentimentAnalysisClient>(client =>
        {
            client.BaseAddress = new Uri("https://twinword-sentiment-analysis.p.rapidapi.com");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-key", section["SubscriptionKey"]);
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-rapidapi-host", "twinword-sentiment-analysis.p.rapidapi.com");
        });

        return services;
    }
}