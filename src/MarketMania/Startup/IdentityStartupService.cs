using MarketMania.Authentication;
using MarketMania.Authentication.Entities;
using Microsoft.AspNetCore.Identity;

namespace MarketMania.Startup;

public class IdentityStartupService(IServiceProvider services, IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        string[] roleNames = [RoleNames.Administrator, RoleNames.PowerUser, RoleNames.User];

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var role = new ApplicationRole(roleName)
                {
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                await roleManager.CreateAsync(role);
            }
        }

        var administratorUserSection = configuration.GetSection("AdministratorUser");
        var powerUserSection = configuration.GetSection("PowerUser");

        var administratorUser = new ApplicationUser
        {
            FirstName = administratorUserSection["FirstName"]!,
            Email = administratorUserSection["Email"],
            UserName = administratorUserSection["Email"]
        };

        var powerUser = new ApplicationUser
        {
            FirstName = powerUserSection["FirstName"]!,
            Email = powerUserSection["Email"],
            UserName = powerUserSection["Email"]
        };

        await RegisterUserAsync(administratorUser, administratorUserSection["Password"]!, RoleNames.Administrator, RoleNames.User);
        await RegisterUserAsync(powerUser, powerUserSection["Password"]!, RoleNames.PowerUser, RoleNames.User);

        async Task RegisterUserAsync(ApplicationUser user, string password, params string[] roles)
        {
            var dbUser = await userManager.FindByNameAsync(user.UserName!);
            if (dbUser is null)
            {
                await userManager.CreateAsync(user, password);
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                await userManager.ConfirmEmailAsync(user, token);
                await userManager.AddToRolesAsync(user, roles);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}