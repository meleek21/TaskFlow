using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PuppeteerSharp;
using StackExchange.Redis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.IdGenerators;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Pdf;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Queue;
using TaskFlow.Infrastructure.Storage;

namespace TaskFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Mapping
        if (!BsonClassMap.IsClassMapRegistered(typeof(TaskFlow.Domain.Entities.Job)))
        {
            BsonClassMap.RegisterClassMap<TaskFlow.Domain.Entities.Job>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id)
                  .SetIdGenerator(StringObjectIdGenerator.Instance)
                  .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        // MongoDB
        var mongoConnectionString = configuration["MongoDB:ConnectionString"] 
            ?? "mongodb://admin:changeme123@localhost:27017?authSource=admin";
        var mongoDatabaseName = configuration["MongoDB:DatabaseName"] ?? "taskflow";
        
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
        services.AddSingleton(mongoDatabase);
        services.AddScoped<IJobRepository, MongoJobRepository>();

        // Redis
        var redisConnection = configuration["Redis:Connection"] ?? "localhost:6379";
        var redis = ConnectionMultiplexer.Connect(redisConnection);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddScoped<IQueueService, RedisQueueService>();

        // Local Storage
        services.AddScoped<IStorageService, LocalStorageService>();
        Console.WriteLine("✅  Registered Local Storage");

        return services;
    }

    public static IServiceCollection AddWorkerInfrastructure(this IServiceCollection services)
    {
        // Puppeteer (browser will be initialized in Worker startup)
        services.AddScoped<IPdfService, PuppeteerPdfService>();
        return services;
    }

    public static async Task<IBrowser> InitializeBrowserAsync()
    {
        Console.WriteLine("⬇️  Downloading Chrome for Puppeteer (this may take a while)...");
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        Console.WriteLine("✅  Chrome downloaded successfully.");
        
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        });

        return browser;
    }
}
