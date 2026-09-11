using MediatR;

using MySolution.Domain.Enums;

namespace MySolution.Application.Features.ApiKey.Command.GenerateApiKey;

public class GenerateApiKeyRequest : IRequest<GenerateApiKeyResponse>
{
    public string Name { get; set; } =  string.Empty;
    public int NumofDaysExpires { get; set; } 
    public List<ApiKeyPermissionType> Permissions { get; set; } = [];
}