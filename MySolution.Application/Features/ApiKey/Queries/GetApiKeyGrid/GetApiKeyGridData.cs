namespace MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

public class GetApiKeyGridData
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public Guid ApplicationId { get; set; }

    public string ApplicationName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string KeyPrefix { get; set; } = string.Empty;

    public List<string> Permissions { get; set; } = [];

    public bool IsRevoked { get; set; }

    public bool IsExpired { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }
}