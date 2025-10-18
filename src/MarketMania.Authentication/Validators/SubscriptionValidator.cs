using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SimpleAuthentication.ApiKey;

namespace MarketMania.Authentication.Validators;

public class SubscriptionValidator(AuthenticationDbContext authenticationDbContext, TimeProvider timeProvider) : IApiKeyValidator
{
    public async Task<ApiKeyValidationResult> ValidateAsync(string apiKey)
    {
        var subscription = await authenticationDbContext.Subscriptions.FirstOrDefaultAsync(s => s.ApiKey == apiKey);

        if (subscription is null)
        {
            return ApiKeyValidationResult.Fail("API key is invalid");
        }

        var now = timeProvider.GetUtcNow();
        if (subscription.ValidFrom > now || subscription.ValidTo < now)
        {
            return ApiKeyValidationResult.Fail("Subscription is not active");
        }

        var claims = new List<Claim>
        {
            new Claim(CustomClaimTypes.PermitLimit, subscription.RequestsPerWindow.ToString()),
            new Claim(CustomClaimTypes.Window, subscription.WindowMinutes.ToString())
        };

        return ApiKeyValidationResult.Success(subscription.UserName, claims);
    }
}