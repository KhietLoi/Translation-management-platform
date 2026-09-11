namespace MySolution.Application.Features.Auth.RefreshToken;

/// <summary>
///     Represents a request to refresh an access token using a refresh token.
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}