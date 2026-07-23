using MediatR;

namespace MySolution.Application.Features.Auth.RefreshToken;

/// <summary>
///     Command to refresh the access token using a refresh token.
/// </summary>
public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public RefreshTokenCommand(RefreshTokenRequest payload)
    {
        Payload = payload;
    }

    public RefreshTokenRequest Payload { get; }
}