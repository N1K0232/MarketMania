using MarketMania.Shared.Enums;

namespace MarketMania.Shared.Models;

public record class PdfOptions(string PageSize = "A4", PdfOrientation Orientation = PdfOrientation.Portrait, PdfMargin Margin = null);