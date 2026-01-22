using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Worker;

public class PdfGenerationWorker : BackgroundService
{
    private readonly ILogger<PdfGenerationWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public PdfGenerationWorker(ILogger<PdfGenerationWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PDF Generation Worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
                var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                var pdfService = scope.ServiceProvider.GetRequiredService<IPdfService>();
                var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();

                var jobId = await queueService.DequeueAsync(stoppingToken);

                if (jobId == null)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                _logger.LogInformation("🔍 Dequeued job: {JobId}. Starting processing...", jobId);

                var job = await jobRepository.GetByIdAsync(jobId, stoppingToken);
                if (job == null)
                {
                    _logger.LogWarning("⚠️ Job not found in database: {JobId}", jobId);
                    continue;
                }

                _logger.LogInformation("📄 Fetched job data from MongoDB for job: {JobId}", jobId);

                // Update status to Processing
                job.Status = JobStatus.Processing;
                job.StartedAt = DateTime.UtcNow;
                await jobRepository.UpdateAsync(job, stoppingToken);
                _logger.LogInformation("⚙️ Job status updated to 'Processing' for job: {JobId}", jobId);

                try
                {
                    _logger.LogInformation("🖥️ Generating PDF from HTML content for job: {JobId}...", jobId);
                    var pdfBytes = await pdfService.GenerateFromHtmlAsync(job.HtmlContent, stoppingToken);
                    _logger.LogInformation("✅ PDF generated successfully ({Size} bytes) for job: {JobId}", pdfBytes.Length, jobId);
                    
                    _logger.LogInformation("💾 Saving PDF to local storage for job: {JobId}...", jobId);
                    var fileName = $"job_{job.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
                    var fileUrl = await storageService.UploadAsync(pdfBytes, fileName, stoppingToken);
                    _logger.LogInformation("📁 PDF saved successfully to: {Url}", fileUrl);

                    // Update job as completed
                    job.Status = JobStatus.Completed;
                    job.OutputUrl = fileUrl;
                    job.FinishedAt = DateTime.UtcNow;
                    await jobRepository.UpdateAsync(job, stoppingToken);

                    _logger.LogInformation("🎉 Job COMPLETED: {JobId}", jobId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Job FAILED: {JobId}", jobId);

                    job.Status = JobStatus.Failed;
                    job.ErrorMessage = ex.Message;
                    job.FinishedAt = DateTime.UtcNow;
                    await jobRepository.UpdateAsync(job, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker error");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
