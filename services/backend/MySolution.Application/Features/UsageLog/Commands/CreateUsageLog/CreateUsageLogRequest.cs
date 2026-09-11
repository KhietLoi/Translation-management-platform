namespace MySolution.Application.Features.UsageLog.Commands.CreateUsageLog;

public class CreateUsageLogRequest
{
    public Guid ApiKeyId { get; set; }
    public Guid ApplicationId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public int DurationMs { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}