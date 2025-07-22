using AutoMapper;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Mapping;

public class RatingMapperProfile : Profile
{
    public RatingMapperProfile()
    {
        CreateMap<Entities.Rating, Rating>()
            .ForMember(r => r.User, options => options.MapFrom(r => r.User.UserName))
            .ForMember(r => r.Product, options => options.MapFrom(r => r.Product.Name));

        CreateMap<NewRatingRequest, Entities.Rating>();
    }
}