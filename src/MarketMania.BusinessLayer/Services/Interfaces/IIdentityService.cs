using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface IIdentityService
{
    Task<Result<StreamFileContent>> GetQRCodeAsync(string token, CancellationToken cancellationToken);

    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<Result> LogoutAsync(CancellationToken cancellationToken);

    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);

    Task<Result<AuthResponse>> ValidateTwoFactorAsync(TwoFactorValidationRequest request, CancellationToken cancellationToken);

    Task<Result> VerifyEmailAsync(string token, string secret, CancellationToken cancellationToken);
}