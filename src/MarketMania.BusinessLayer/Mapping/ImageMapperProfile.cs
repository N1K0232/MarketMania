using AutoMapper;
using MarketMania.Shared.Models;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Mapping;

public class ImageMapperProfile : Profile
{
    public ImageMapperProfile()
    {
        CreateMap<Entities.Image, Image>();
    }
}