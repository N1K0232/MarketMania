using MarketMania.Shared.Models;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface IMeService
{
    Task<Result<User>> GetAsync();
}