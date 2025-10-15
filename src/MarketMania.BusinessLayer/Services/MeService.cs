using AutoMapper;
using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OperationResults;
using TinyHelpers.Extensions;

namespace MarketMania.BusinessLayer.Services;

public class MeService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : IMeService
{
    public async Task<Result<User>> GetAsync()
    {
        var userName = httpContextAccessor.HttpContext?.User.Identity?.Name;
        if (userName.HasValue())
        {
            var dbUser = await userManager.FindByNameAsync(userName);
            var user = mapper.Map<User>(dbUser);

            return user;
        }

        return Result.Fail(FailureReasons.Unauthorized);
    }
}