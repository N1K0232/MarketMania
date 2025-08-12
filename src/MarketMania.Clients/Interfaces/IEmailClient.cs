using MarketMania.Clients.Models.Email;

namespace MarketMania.Clients.Interfaces;

public interface IEmailClient : IAsyncDisposable
{
    Task<SendEmailResponse> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}