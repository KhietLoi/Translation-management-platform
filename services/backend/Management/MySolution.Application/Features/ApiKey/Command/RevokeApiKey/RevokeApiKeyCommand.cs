using MediatR;

namespace MySolution.Application.Features.ApiKey.Command.RevokeApiKey;

public class RevokeApiKeyCommand : IRequest<RevokeApiKeyResponse>
{
    public Guid ApiKeyId { get; set; }
    public RevokeApiKeyCommand(Guid apiKeyId)
    {
        ApiKeyId = apiKeyId;
    }
}