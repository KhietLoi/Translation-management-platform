using MediatR;

namespace MySolution.Application.Features.ApiKey.Command.GenerateApiKey;

public class GenerateApiKeyRequest : IRequest<GenerateApiKeyResponse>
{
    public string Name { get; set; } =  string.Empty;
    public int NumofDaysExpires { get; set; } 
}