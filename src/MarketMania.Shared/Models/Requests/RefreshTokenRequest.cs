using System;
using System.Collections.Generic;
using System.Text;

namespace MarketMania.Shared.Models.Requests;

public record class RefreshTokenRequest(string AccessToken, string RefreshToken);