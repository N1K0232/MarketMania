using System.Text.Json;

namespace MarketMania.BusinessLayer.Exceptions;

public class TranslatorClientException(int code, string message) : Exception(message)
{
    public int Code { get; } = code;

    internal static async Task<TranslatorClientException> ReadFromResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        using var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

        try
        {
            using var jsonDocument = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
            var error = jsonDocument.RootElement.GetProperty("error");

            var code = Convert.ToInt32(error.GetProperty("code").GetString());
            var message = error.GetProperty("message").GetString();

            return new TranslatorClientException(code, message);
        }
        catch
        {
            responseStream.Position = 0;
            using var reader = new StreamReader(responseStream);

            var message = await reader.ReadToEndAsync(cancellationToken) ?? "Unknown error";
            return new TranslatorClientException(500, message);
        }
    }
}