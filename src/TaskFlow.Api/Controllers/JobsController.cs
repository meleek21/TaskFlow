using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobRepository _jobRepository;
    private readonly IQueueService _queueService;

    public JobsController(IJobRepository jobRepository, IQueueService queueService)
    {
        _jobRepository = jobRepository;
        _queueService = queueService;
    }

    /// <summary>
    /// Submit a new PDF generation job
    /// </summary>
    [HttpPost("pdf")]
    public async Task<ActionResult<JobResponse>> CreateJob([FromBody] CreateJobRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TemplateHtml))
        {
            return BadRequest(new { error = "TemplateHtml is required" });
        }

        var job = new Job
        {
            HtmlContent = request.TemplateHtml,
            Data = request.Data,
            Metadata = request.Metadata != null
                ? new JobMetadata
                {
                    SourceApp = request.Metadata.SourceApp,
                    ReferenceId = request.Metadata.ReferenceId
                }
                : null
        };

        var createdJob = await _jobRepository.CreateAsync(job, cancellationToken);
        await _queueService.EnqueueAsync(createdJob.Id, cancellationToken);

        return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, new JobResponse
        {
            JobId = createdJob.Id,
            Status = createdJob.Status
        });
    }

    /// <summary>
    /// Get job status by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<JobResponse>> GetJob(string id, CancellationToken cancellationToken)
    {
        id = id?.Trim() ?? string.Empty;
        var job = await _jobRepository.GetByIdAsync(id, cancellationToken);

        if (job == null)
        {
            return NotFound(new { error = "Job not found" });
        }

        return Ok(new JobResponse
        {
            JobId = job.Id,
            Status = job.Status,
            FileUrl = job.OutputUrl,
            ErrorMessage = job.ErrorMessage
        });
    }
}
