namespace TaskFlow.Application.Interfaces;

public interface IQueueService
{
    Task EnqueueAsync(string jobId, CancellationToken cancellationToken = default);
    Task<string?> DequeueAsync(CancellationToken cancellationToken = default);
}
