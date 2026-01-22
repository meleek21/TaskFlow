using StackExchange.Redis;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Infrastructure.Queue;

public class RedisQueueService : IQueueService
{
    private readonly IDatabase _redis;
    private const string QueueKey = "pdf:jobs:queue";

    public RedisQueueService(IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }

    public async Task EnqueueAsync(string jobId, CancellationToken cancellationToken = default)
    {
        await _redis.ListRightPushAsync(QueueKey, jobId);
    }

    public async Task<string?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        var value = await _redis.ListLeftPopAsync(QueueKey);
        return value.HasValue ? value.ToString() : null;
    }
}
