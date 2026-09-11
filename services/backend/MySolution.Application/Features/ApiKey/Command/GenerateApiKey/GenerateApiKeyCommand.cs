using MediatR;

namespace MySolution.Application.Features.ApiKey.Command.GenerateApiKey;

public class GenerateApiKeyCommand : IRequest<GenerateApiKeyResponse>
{
    public GenerateApiKeyRequest Payload { get; set; }
    public Guid ApplicationId { get; set; }

    public GenerateApiKeyCommand(GenerateApiKeyRequest payload,  Guid applicationId)
    {
        Payload = payload;
        ApplicationId = applicationId;
    }
}