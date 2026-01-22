using Microsoft.Extensions.FileProviders;
using TaskFlow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load .env file if exists
DotNetEnv.Env.TraversePath().Load();

// Add configuration from environment variables
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddCommonInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

// Serve PDFs from the configured path
var storagePath = builder.Configuration["Storage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
if (!Directory.Exists(storagePath))
{
    Directory.CreateDirectory(storagePath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storagePath),
    RequestPath = "/pdfs"
});

app.MapControllers();

app.Run();
