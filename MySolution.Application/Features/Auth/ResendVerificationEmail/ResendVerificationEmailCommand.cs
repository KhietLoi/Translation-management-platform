using MediatR;
using MySolution.Application.Common.Interfaces.RateLimit;


namespace MySolution.Application.Features.Auth.ResendVerificationEmail;

public class ResendVerificationEmailCommand : IRequest<ResendVerificationEmailResponse>, IRateLimitedRequest
{
    public ResendVerificationEmailCommand(ResendVerificationEmailRequest payload)
    {
        Payload = payload;
    }

    public ResendVerificationEmailRequest Payload { get; }
    public string PolicyName => "ResendEmail";
    public string RateLimitKey => $"resend-email:{Payload.Email}";
}