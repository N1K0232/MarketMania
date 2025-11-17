using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Generators.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using QRCoder;

namespace MarketMania.BusinessLayer.Generators;

public class QRCodeHandlerGenerator(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment) : IQRCodeGenerator
{
    public async Task<Stream> GenerateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        await userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        var secret = await userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);

        var qrCodeUri = $"otpauth://totp/{Uri.EscapeDataString(environment.ApplicationName)}:{user.Email}?secret={secret}&issuer={Uri.EscapeDataString(environment.ApplicationName)}";
        using var generator = new QRCodeGenerator();

        using var qrCodeData = generator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);

        var qrCodeBytes = qrCode.GetGraphic(3);
        return new MemoryStream(qrCodeBytes);
    }
}