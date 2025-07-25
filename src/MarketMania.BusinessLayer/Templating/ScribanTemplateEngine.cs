using System.Globalization;
using System.Text.RegularExpressions;
using MarketMania.BusinessLayer.Exceptions;
using MarketMania.BusinessLayer.Providers;
using MarketMania.BusinessLayer.Templating.Interfaces;
using Scriban;
using Scriban.Runtime;

namespace MarketMania.BusinessLayer.Templating;

public partial class ScribanTemplateEngine(TimeZoneTimeProvider timeZoneTimeProvider) : ITemplateEngine
{
    private const string DateTimeZonePlaceholder = "datetime_withzone";

    public async Task<string> RenderAsync(string template, object model, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        var sanitizedTemplate = DateNowRegex.Replace(template, DateTimeZonePlaceholder);
        var generatedTemplate = Template.Parse(template);

        if (generatedTemplate.HasErrors)
        {
            throw new TemplateEngineException(generatedTemplate.Messages.ToString());
        }

        var context = new TemplateContext
        {
            MemberRenamer = member => member.Name
        };

        context.PushGlobal(new ScriptObject { { "Model", model } });
        context.PushCulture(culture);

        var dateWithTimeZoneScript = new ScriptObject();
        dateWithTimeZoneScript.Import(DateTimeZonePlaceholder, new Func<DateTime>(() => timeZoneTimeProvider.GetLocalNow().DateTime));
        context.PushGlobal(dateWithTimeZoneScript);

        var result = await generatedTemplate.RenderAsync(context);
        return result;
    }

    [GeneratedRegex("(?<![\\w$])date\\.now(?![\\w$])")]
    private static partial Regex DateNowRegex { get; }
}