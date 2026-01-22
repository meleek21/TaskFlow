using PuppeteerSharp;
using PuppeteerSharp.Media;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Infrastructure.Pdf;

public class PuppeteerPdfService : IPdfService
{
    private readonly IBrowser _browser;

    public PuppeteerPdfService(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<byte[]> GenerateFromHtmlAsync(string htmlContent, CancellationToken cancellationToken = default)
    {
        await using var page = await _browser.NewPageAsync();
        await page.SetContentAsync(htmlContent);
        
        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "20px",
                Bottom = "20px",
                Left = "20px",
                Right = "20px"
            }
        });

        return pdfBytes ?? Array.Empty<byte>();
    }
}
