using MediatR;


namespace MySolution.Application.Features.Auth.ResendVerificationEmail;

public class ResendVerificationEmailCommand : IRequest<ResendVerificationEmailResponse>
{
    public ResendVerificationEmailCommand(ResendVerificationEmailRequest payload)
    {
        Payload = payload;
    }

    public ResendVerificationEmailRequest Payload { get; }
}