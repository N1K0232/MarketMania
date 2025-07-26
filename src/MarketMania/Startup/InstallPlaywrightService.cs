using MarketMania.HealthChecks;

namespace MarketMania.Startup;

public class InstallPlaywrightService(PlaywrightHealthCheck playwrightHealthCheck, ILogger<InstallPlaywrightService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var returnCode = await Task.Run(() =>
        {
            try
            {
                return Microsoft.Playwright.Program.Main(["install", "chromium"]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while installing Chromium");
                return -1;
            }
        }, stoppingToken);

        var playwrightStatus = returnCode switch
        {
            0 => PlaywrightStatus.Installed,
            _ => PlaywrightStatus.Error
        };

        playwrightHealthCheck.Status = playwrightStatus;
    }
}