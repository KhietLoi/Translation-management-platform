using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.ApiKey.Command.GenerateApiKey;

public class GenerateApiKeyResponse : BaseResponse  <GenerateApiKeyData>
{
    
}

public class GenerateApiKeyData
{
    public Guid Id { get; set; } 
    public Guid ApplicationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}