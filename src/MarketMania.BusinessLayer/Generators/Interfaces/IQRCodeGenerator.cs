using System;
using System.Collections.Generic;
using System.Text;
using MarketMania.Authentication.Entities;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface IQRCodeGenerator
{
    Task<Stream> GenerateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}