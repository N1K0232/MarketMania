using AutoMapper;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Mapping;

public class BrandMapperProfile : Profile
{
    public BrandMapperProfile()
    {
        CreateMap<Entities.Brand, Brand>();
        CreateMap<SaveBrandRequest, Entities.Brand>();
    }
}