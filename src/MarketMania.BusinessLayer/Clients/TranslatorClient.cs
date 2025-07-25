using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MarketMania.BusinessLayer.Clients.Interfaces;
using MarketMania.BusinessLayer.Exceptions;
using MarketMania.BusinessLayer.Resources;
using MarketMania.BusinessLayer.Settings;
using MarketMania.Shared.Models.Translations;
using Microsoft.Extensions.Options;
using TinyHelpers.Extensions;

namespace MarketMania.BusinessLayer.Clients;

public class TranslatorClient(HttpClient httpClient, IAzureAuthTokenClient azureAuthTokenClient, IOptions<TranslatorSettings> translatorSettingsOptions) : ITranslatorClient
{
    private static readonly JsonSerializerOptions jsonSerializerOptions;

    private readonly TranslatorSettings translatorSettings = translatorSettingsOptions.Value;
    private string authorizationHeaderValue = string.Empty;

    static TranslatorClient()
    {
        jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<IEnumerable<DetectedLanguageResponse>> DetectedLanguagesAsync(IEnumerable<string> input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));

        await CheckUpdateTokenAsync(cancellationToken);
        var uriString = $"{TranslationResources.BaseUrl}detect?{TranslationResources.ApiVersion}";

        using var httpRequest = CreateHttpRequest(uriString, HttpMethod.Post, input.Select(t => new
        {
            Text = t[..Math.Min(t.Length, Constants.MaxTextLengthForDetection)]
        }));

        using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (httpResponse.IsSuccessStatusCode)
        {
            var responseContent = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<DetectedLanguageResponse>>(cancellationToken);
            return responseContent;
        }

        throw await TranslatorClientException.ReadFromResponseAsync(httpResponse, cancellationToken);
    }

    public async Task<IEnumerable<ServiceLanguage>> GetLanguagesAsync(string language = null, CancellationToken cancellationToken = default)
    {
        await CheckUpdateTokenAsync(cancellationToken);
        var uriString = $"languages?scope=translation&{TranslationResources.ApiVersion}";

        using var httpRequest = CreateHttpRequest(uriString);
        var requestedLanguage = language.GetValueOrDefault(translatorSettings.DefaultLanguage);

        if (requestedLanguage.HasValue())
        {
            httpRequest.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(requestedLanguage));
        }

        using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (httpResponse.IsSuccessStatusCode)
        {
            using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
            using var jsonDocument = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var jsonContent = jsonDocument.RootElement.GetProperty("translation");
            var responseContent = JsonSerializer.Deserialize<Dictionary<string, ServiceLanguage>>(jsonContent.ToString(), jsonSerializerOptions).ToList();

            return responseContent.Select(r => r.Value).OrderBy(r => r.Name).ToList();
        }

        throw await TranslatorClientException.ReadFromResponseAsync(httpResponse, cancellationToken);
    }

    private async Task CheckUpdateTokenAsync(CancellationToken cancellationToken)
    {
        authorizationHeaderValue = await azureAuthTokenClient.GetAccessTokenAsync(cancellationToken);
    }

    private HttpRequestMessage CreateHttpRequest(string uriString)
        => CreateHttpRequest(uriString, HttpMethod.Get);

    private HttpRequestMessage CreateHttpRequest(string uriString, HttpMethod method, object content = null)
    {
        var request = new HttpRequestMessage(method, new Uri(uriString))
        {
            Content = content != null ? JsonContent.Create(content, content.GetType(), options: jsonSerializerOptions) : null
        };

        request.Headers.Add(Constants.AuthorizationHeader, authorizationHeaderValue);
        return request;
    }
}