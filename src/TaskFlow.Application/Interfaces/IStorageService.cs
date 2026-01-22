namespace TaskFlow.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(byte[] fileBytes, string fileName, CancellationToken cancellationToken = default);
}
