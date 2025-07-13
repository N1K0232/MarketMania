using Microsoft.AspNetCore.Hosting;
using QRCoder;

namespace MarketMania.Authentication.Generators;

public class QRCodeHandlerGenerator(IWebHostEnvironment environment) : IQRCodeGenerator
{
    public Task<Stream> GenerateAsync(string email, string secret, CancellationToken cancellationToken = default)
    {
        var qrCodeUri = $"otpauth://totp/{Uri.EscapeDataString(environment.ApplicationName)}:{email}?secret={secret}&issuer={Uri.EscapeDataString(environment.ApplicationName)}";
        using var generator = new QRCodeGenerator();

        using var qrCodeData = generator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);

        var qrCodeBytes = qrCode.GetGraphic(3);
        var stream = new MemoryStream(qrCodeBytes);

        return Task.FromResult<Stream>(stream);
    }
}