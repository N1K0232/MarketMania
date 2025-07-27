
using MarketMania.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.Startup;

public class DatabaseInitializerService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await applicationDbContext.Database.EnsureCreatedAsync(cancellationToken);
        await applicationDbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}