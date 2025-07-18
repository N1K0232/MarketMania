using AutoMapper;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Mapping;

public class SupplierMapperProfile : Profile
{
    public SupplierMapperProfile()
    {
        CreateMap<Entities.Supplier, Supplier>();
        CreateMap<SaveSupplierRequest, Entities.Supplier>();
    }
}