using MediatR;

namespace MySolution.Application.Features.Auth.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public RefreshTokenRequest Payload { get; set; } = new();
}