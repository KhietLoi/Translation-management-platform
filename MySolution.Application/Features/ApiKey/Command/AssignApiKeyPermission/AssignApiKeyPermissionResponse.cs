using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.ApiKey.Command.AssignApiKeyPermission;

public class AssignApiKeyPermissionResponse : BaseResponse <AssignApiKeyPermissionData>
{
}
public class AssignApiKeyPermissionData
{
    public Guid ApiKeyId { get; set; }
    public List<ApiKeyPermissionType> Permissions { get; set; } = new();
}