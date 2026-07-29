using MediatR;
using MySolution.Application.Common.Interfaces.RateLimit;

namespace MySolution.Application.Features.Admin.User.Commands.ResendSetupPassword;

public class ResendSetupPasswordCommand : IRequest<ResendSetupPasswordResponse>, IRateLimitedRequest
{
    
    public ResendSetupPasswordCommand(ResendSetupPasswordRequest payload)
    {
        Payload = payload;
    }
    public ResendSetupPasswordRequest Payload { get; set; }
    public string PolicyName => "ResendSetupPassword";
    public string RateLimitKey => $"resend-setup-password-{Payload.Email}";
}