namespace MarketMania.Clients.Models.Pdf;

public record class PdfOptions(string PageSize = "A4", PdfOrientation Orientation = PdfOrientation.Portrait, PdfMargin Margin = null);