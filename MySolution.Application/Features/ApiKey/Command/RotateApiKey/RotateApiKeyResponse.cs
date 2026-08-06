using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.ApiKey.Command.RotateApiKey;

public class RotateApiKeyResponse : BaseResponse <RotateApiKeyData>
{
}

public class RotateApiKeyData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}