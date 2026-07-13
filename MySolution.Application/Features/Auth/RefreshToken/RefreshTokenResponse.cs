using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Auth.RefreshToken;

/// <summary>
/// RefreshTokenResponse is a response class that represents the result of a refresh token operation.
/// </summary>
public class RefreshTokenResponse : BaseResponse <RefreshTokenResult>
{
}
public class RefreshTokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiredAt { get; set; } 
}