using MediatR;
using MySolution.Application.Common.Interfaces.RateLimit;
using MySolution.Application.Common.Models.RateLimit;

namespace MySolution.Application.Features.Auth.ResendVerificationEmail;

public class ResendVerificationEmailCommand : IRequest<ResendVerificationEmailResponse>, IRateLimitedRequest
{
    public ResendVerificationEmailCommand(ResendVerificationEmailRequest payload)
    {
        Payload = payload;
    }

    public ResendVerificationEmailRequest Payload { get; }

    public RateLimitPolicy GetRateLimitPolicy()
    {
        return new RateLimitPolicy($"resend-email:{Payload.Email}",
            3,
            TimeSpan.FromMinutes(10)
        );
    }
}