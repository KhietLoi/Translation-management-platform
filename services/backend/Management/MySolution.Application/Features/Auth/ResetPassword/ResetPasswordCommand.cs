using MediatR;

namespace MySolution.Application.Features.Auth.ResetPassword;

public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
{
    public ResetPasswordRequest Payload { get; set; }
    public ResetPasswordCommand(ResetPasswordRequest payload)
    {
        Payload = payload;
    }
}