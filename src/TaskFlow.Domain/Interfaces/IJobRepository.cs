using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

public interface IJobRepository
{
    Task<Job> CreateAsync(Job job, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
}
