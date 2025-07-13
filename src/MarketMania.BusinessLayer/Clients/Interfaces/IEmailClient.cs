using MarketMania.Shared.Models;

namespace MarketMania.BusinessLayer.Clients.Interfaces;

public interface IEmailClient : IAsyncDisposable
{
    Task<SendEmailResponse> SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}