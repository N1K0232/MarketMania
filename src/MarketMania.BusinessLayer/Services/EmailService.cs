using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Models.Email;
using OperationResults;

namespace MarketMania.BusinessLayer.Services;

public class EmailService(IEmailClient emailClient) : IEmailService
{
    public async Task<Result> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        var response = await emailClient.SendAsync(emailMessage, cancellationToken);
        if (response.Succeed)
        {
            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ClientError, "Unable to send the email");
    }
}