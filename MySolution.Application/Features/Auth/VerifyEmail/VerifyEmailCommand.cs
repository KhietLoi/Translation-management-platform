using MediatR;

namespace MySolution.Application.Features.Auth.VerifyEmail;

public class VerifyEmailCommand : IRequest <VerifyEmailResponse>
{
    public string Token { get; set; }
}