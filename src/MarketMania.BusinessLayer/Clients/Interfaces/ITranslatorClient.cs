using MarketMania.Shared.Models.Translations;

namespace MarketMania.BusinessLayer.Clients.Interfaces;

public interface ITranslatorClient
{
    async Task<DetectedLanguageResponse> DetectLanguageAsync(string input, CancellationToken cancellationToken = default)
    {
        var response = await DetectedLanguagesAsync([input], cancellationToken);
        return response.FirstOrDefault();
    }

    Task<IEnumerable<DetectedLanguageResponse>> DetectedLanguagesAsync(IEnumerable<string> input, CancellationToken cancellationToken = default);

    Task<IEnumerable<ServiceLanguage>> GetLanguagesAsync(string language = null, CancellationToken cancellationToken = default);


}