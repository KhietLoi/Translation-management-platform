namespace MySolution.Domain.Entities;

public class ApiKeyUsageLog
{
    public Guid Id { get; set; }
    public Guid ApiKeyId { get; set; }
    public string Endpoint { get; set; } = null!;
    public string Method { get; set; } = null!;
    public int StatusCode { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ApiKey ApiKey { get; set; } = null!;
}