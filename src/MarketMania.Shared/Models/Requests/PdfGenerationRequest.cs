using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketMania.Shared.Models.Requests;

[method: JsonConstructor]
public record class PdfGenerationRequest(string Template, JsonDocument Model, PdfOptions Options, string TemplateEngine = "scriban", string FileName = null)
{
    public PdfGenerationRequest(string template, object model, PdfOptions options, string templateEngine = "scriban", string fileName = null)
        : this(template, ToJsonDocument(model), options, templateEngine, fileName)
    {
    }

    private static JsonDocument ToJsonDocument(object model)
    {
        if (model is null)
        {
            return null;
        }

        var jsonString = JsonSerializer.Serialize(model, JsonSerializerOptions.Default);
        return JsonDocument.Parse(jsonString);
    }
}