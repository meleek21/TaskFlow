namespace TaskFlow.Application.DTOs;

public class CreateJobRequest
{
    public string TemplateHtml { get; set; } = null!;
    public Dictionary<string, object>? Data { get; set; }
    public JobMetadataDto? Metadata { get; set; }
}

public class JobMetadataDto
{
    public string? SourceApp { get; set; }
    public string? ReferenceId { get; set; }
}
