using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.ApiKey.Command.RotateApiKey;

public class RotateApiKeyResponse : BaseResponse <RotateApiKeyData>
{
}

public class RotateApiKeyData
{
    public Guid ApiKeyId { get; set; }
    public string ApiKey { get; set; } = string.Empty;
}