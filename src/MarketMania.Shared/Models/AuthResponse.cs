namespace MarketMania.Shared.Models;

public class AuthResponse
{
    public AuthResponse(string twoFactorToken)
    {
        TwoFactorToken = twoFactorToken;
    }

    public AuthResponse(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string? TwoFactorToken { get; }

    public string? AccessToken { get; }

    public string? RefreshToken { get; }
}