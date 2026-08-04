namespace MySolution.Domain.Entities;

public class ApiKey
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Name { get; set; } = null!;
    public string KeyHash { get; set; } = null!;
    public string KeyPrefix { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Application Application { get; set; } = null!;
    public ICollection<ApiKeyPermission> Permissions = new List<ApiKeyPermission>();
    public ICollection<ApiKeyUsageLog> UsageLogs = new List<ApiKeyUsageLog>();
}