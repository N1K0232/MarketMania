using AutoMapper;
using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OperationResults;

namespace MarketMania.BusinessLayer.Services;

public class MeService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper) : IMeService
{
    public async Task<Result<User>> GetAsync()
    {
        var userName = httpContextAccessor.HttpContext.User.Identity.Name;
        var dbUser = await userManager.FindByNameAsync(userName);

        var user = mapper.Map<User>(dbUser);
        return user;
    }
}