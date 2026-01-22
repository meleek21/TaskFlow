using Microsoft.Extensions.Configuration;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Infrastructure.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string _storagePath;
    private readonly string _baseUrl;

    public LocalStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["Storage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
        _baseUrl = configuration["Storage:BaseUrl"] ?? "http://localhost:5095/pdfs/";

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> UploadAsync(byte[] fileBytes, string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);
        
        return $"{_baseUrl.TrimEnd('/')}/{fileName}";
    }
}
