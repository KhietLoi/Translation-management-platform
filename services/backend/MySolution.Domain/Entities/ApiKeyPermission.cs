using MySolution.Domain.Enums;

namespace MySolution.Domain.Entities;

public class ApiKeyPermission
{
    public Guid Id { get; set; }
    public Guid ApiKeyId { get; set; }
    public ApiKeyPermissionType Permission { get; set; }
    public virtual ApiKey ApiKey { get; set; } = null!;
}