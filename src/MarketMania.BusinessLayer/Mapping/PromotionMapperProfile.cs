using AutoMapper;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Mapping;

public class PromotionMapperProfile : Profile
{
    public PromotionMapperProfile()
    {
        CreateMap<Entities.Promotion, Promotion>();
        CreateMap<SavePromotionRequest, Entities.Promotion>();
    }
}