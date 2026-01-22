using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public class Job
{
    public string Id { get; set; } = null!;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string HtmlContent { get; set; } = null!;
    public Dictionary<string, object>? Data { get; set; }
    public JobMetadata? Metadata { get; set; }
    public string? OutputUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}

public class JobMetadata
{
    public string? SourceApp { get; set; }
    public string? ReferenceId { get; set; }
}
