using System.Globalization;

namespace MarketMania.BusinessLayer.Templating.Interfaces;

public interface ITemplateEngine
{
    Task<string> RenderAsync(string template, object model, CultureInfo culture, CancellationToken cancellationToken = default);
}