using MediatR;
using MySolution.Application.Common.Interfaces.RateLimit;

namespace MySolution.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>, IRateLimitedRequest
{
    public ForgotPasswordCommand(ForgotPasswordRequest payload)
    {
        Payload = payload;
    }

    public ForgotPasswordRequest Payload { get; set; }
    public string PolicyName  => "ForgotPassword";
    public string RateLimitKey => $"forgot-password:{Payload.Email}";
}