using MediatR;

namespace MySolution.Application.Features.Auth.ResendVerificationEmail;

public class ResendVerificationEmailCommand : IRequest<ResendVerificationEmailResponse>
{
    public ResendVerificationEmailRequest Payload { get; }

    public ResendVerificationEmailCommand(ResendVerificationEmailRequest payload)
    {
        Payload = payload;
    }
}