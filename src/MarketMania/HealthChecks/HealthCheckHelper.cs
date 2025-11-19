using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MarketMania.HealthChecks;

public static class HealthCheckHelper
{
    public static Func<HttpContext, HealthReport, Task> HealthChecksResponseWriter() => WriteAsync;

    private static async Task WriteAsync(HttpContext context, HealthReport report)
    {
        var result = JsonSerializer.Serialize(
            new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds,
                details = report.Entries.Select(entry => new
                {
                    service = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    exception = entry.Value.Exception?.Message,
                })
            });

        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(result);
    }
}