using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs;

public class JobResponse
{
    public string JobId { get; set; } = null!;
    public JobStatus Status { get; set; }
    public string? FileUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
