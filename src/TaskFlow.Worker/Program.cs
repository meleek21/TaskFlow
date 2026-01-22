using PuppeteerSharp;
using TaskFlow.Infrastructure;
using TaskFlow.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Load .env file if exists
DotNetEnv.Env.TraversePath().Load();

// Add configuration from environment variables
builder.Configuration.AddEnvironmentVariables();

// Add infrastructure services
builder.Services.AddCommonInfrastructure(builder.Configuration);
builder.Services.AddWorkerInfrastructure();

// Initialize Puppeteer browser and register it
var browser = await DependencyInjection.InitializeBrowserAsync();
builder.Services.AddSingleton<IBrowser>(browser);

// Add worker service
builder.Services.AddHostedService<PdfGenerationWorker>();

var host = builder.Build();
host.Run();
