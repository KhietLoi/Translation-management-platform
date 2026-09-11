namespace MySolution.Domain.Entities;

public class ApiKeyUsageLog
{
    public Guid Id { get; set; }
    public Guid ApiKeyId { get; set; }
    public Guid ApplicationId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public int DurationMs { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public ApiKey ApiKey { get; set; } = null!;
    public ApiKey Application { get; set; } = null!;
}