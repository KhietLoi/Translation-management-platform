using MySolution.Domain.Enums;

namespace MySolution.Application.Features.ApiKey.Command.AssignApiKeyPermission;

public class AssignApiKeyPermissionRequest
{
    public List<ApiKeyPermissionType> Permissions { get; set; } = new();
    
}