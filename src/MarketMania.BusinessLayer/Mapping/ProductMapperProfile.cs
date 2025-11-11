using AutoMapper;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Mapping;

public class ProductMapperProfile : Profile
{
    public ProductMapperProfile()
    {
        CreateMap<Entities.Product, Product>()
            .ForMember(p => p.Brand, options => options.MapFrom(p => p.Brand.Name))
            .ForMember(p => p.Category, options => options.MapFrom(p => p.Category.Name))
            .ForMember(p => p.Supplier, options => options.MapFrom(p => p.Supplier.Name));

        CreateMap<SaveProductRequest, Entities.Product>();
    }
}