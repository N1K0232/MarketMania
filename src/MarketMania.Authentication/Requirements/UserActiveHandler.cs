using System.Security.Claims;
using MarketMania.Authentication.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SimpleAuthentication;

namespace MarketMania.Requirements;

public class UserActiveHandler(UserManager<ApplicationUser> userManager) : AuthorizationHandler<UserActiveRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserActiveRequirement requirement)
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user is not null)
        {
            var lockedOut = await userManager.IsLockedOutAsync(user);
            var securityStamp = context.User.GetClaimValue(ClaimTypes.SerialNumber);

            if (!lockedOut && securityStamp == user.SecurityStamp)
            {
                context.Succeed(requirement);
            }
        }
    }
}