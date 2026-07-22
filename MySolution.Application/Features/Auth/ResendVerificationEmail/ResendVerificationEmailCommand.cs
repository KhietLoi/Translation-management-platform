using MediatR;
using MySolution.Application.Common.Interfaces.RateLimit;
using MySolution.Application.Common.Models.RateLimit;

namespace MySolution.Application.Features.Auth.ResendVerificationEmail;

public class ResendVerificationEmailCommand : IRequest<ResendVerificationEmailResponse>, IRateLimitedRequest
{
    public ResendVerificationEmailRequest Payload { get; }

    public ResendVerificationEmailCommand(ResendVerificationEmailRequest payload)
    {
        Payload = payload;
    }
  
    public RateLimitPolicy GetRateLimitPolicy()
    {
        return new RateLimitPolicy(Key:
            $"resend-email:{Payload.Email}",
            PermitLimit: 3,
            Window: TimeSpan.FromMinutes(10)
        );
    }
}