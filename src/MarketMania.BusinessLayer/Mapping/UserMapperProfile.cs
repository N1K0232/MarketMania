using AutoMapper;
using MarketMania.Authentication.Entities;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using TinyHelpers.Extensions;

namespace MarketMania.BusinessLayer.Mapping;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<RegisterRequest, ApplicationUser>()
            .ForMember(user => user.UserName, options => options.MapFrom(request => request.UserName.GetValueOrDefault(request.Email)));

        CreateMap<ApplicationUser, User>();
    }
}