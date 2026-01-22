namespace TaskFlow.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateFromHtmlAsync(string htmlContent, CancellationToken cancellationToken = default);
}
