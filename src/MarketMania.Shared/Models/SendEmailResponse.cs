namespace MarketMania.Shared.Models;

public record class SendEmailResponse(bool Succeed, string ErrorMessage = null);