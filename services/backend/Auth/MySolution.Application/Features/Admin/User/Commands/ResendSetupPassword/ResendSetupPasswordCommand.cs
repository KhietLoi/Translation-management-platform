using MediatR;

namespace MySolution.Application.Features.Admin.User.Commands.ResendSetupPassword;

public class ResendSetupPasswordCommand : IRequest<ResendSetupPasswordResponse>
{
    
    public ResendSetupPasswordCommand(ResendSetupPasswordRequest payload)
    {
        Payload = payload;
    }
    public ResendSetupPasswordRequest Payload { get; set; }
    public string PolicyName => "ResendSetupPassword";
    public string RateLimitKey => $"resend-setup-password-{Payload.Email}";
}