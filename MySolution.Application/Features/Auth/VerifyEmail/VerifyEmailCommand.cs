using MediatR;

namespace MySolution.Application.Features.Auth.VerifyEmail;

public class VerifyEmailCommand : IRequest<VerifyEmailResponse>
{
    public VerifyEmailCommand(string token)
    {
        Token = token;
    }

    public string Token { get; set; } = string.Empty;
}