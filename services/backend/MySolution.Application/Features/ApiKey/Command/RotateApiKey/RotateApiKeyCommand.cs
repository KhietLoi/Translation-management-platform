using MediatR;

namespace MySolution.Application.Features.ApiKey.Command.RotateApiKey;

public class RotateApiKeyCommand : IRequest<RotateApiKeyResponse>
{
    public Guid ApiKeyId { get; set; }

    public RotateApiKeyCommand(Guid apiKeyId)
    {
        ApiKeyId = apiKeyId;
    }
}