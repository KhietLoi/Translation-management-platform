using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Models;

public class ApiKeyContext
{
    public Guid ApiKeyId { get; set; }
    public Guid ApplicationId { get; set; }
    public List<ApiKeyPermissionType> Permissions { get; set; } = [];
    
}