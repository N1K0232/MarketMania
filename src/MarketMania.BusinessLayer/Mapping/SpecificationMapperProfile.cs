using AutoMapper;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Mapping;

public class SpecificationMapperProfile : Profile
{
    public SpecificationMapperProfile()
    {
        CreateMap<Entities.Specification, Specification>();
        CreateMap<SaveSpecificationRequest, Entities.Specification>();
    }
}