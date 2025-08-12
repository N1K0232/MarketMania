using MarketMania.Clients.Models.Email;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface IEmailService
{
    Task<Result> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken);
}