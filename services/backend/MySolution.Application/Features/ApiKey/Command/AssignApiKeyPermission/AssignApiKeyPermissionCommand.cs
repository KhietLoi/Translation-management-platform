using MediatR;

namespace MySolution.Application.Features.ApiKey.Command.AssignApiKeyPermission;

public class AssignApiKeyPermissionCommand : IRequest<AssignApiKeyPermissionResponse>
{
    public AssignApiKeyPermissionRequest Payload { get; set; }
    public Guid ApiKeyId { get; set; }

    public AssignApiKeyPermissionCommand(AssignApiKeyPermissionRequest payload,  Guid apiKeyId)
    {
        Payload = payload;
        ApiKeyId = apiKeyId;
    }
}