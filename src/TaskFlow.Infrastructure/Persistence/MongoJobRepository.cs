using MongoDB.Driver;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Persistence;

public class MongoJobRepository : IJobRepository
{
    private readonly IMongoCollection<Job> _jobs;

    public MongoJobRepository(IMongoDatabase database)
    {
        _jobs = database.GetCollection<Job>("jobs");
    }

    public async Task<Job> CreateAsync(Job job, CancellationToken cancellationToken = default)
    {
        job.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await _jobs.InsertOneAsync(job, cancellationToken: cancellationToken);
        return job;
    }

    public async Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Job>.Filter.Eq(j => j.Id, id?.Trim() ?? string.Empty);
        return await _jobs.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Job>.Filter.Eq(j => j.Id, job.Id);
        await _jobs.ReplaceOneAsync(filter, job, cancellationToken: cancellationToken);
    }
}
