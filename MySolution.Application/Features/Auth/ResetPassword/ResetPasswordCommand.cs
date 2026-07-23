using MediatR;

namespace MySolution.Application.Features.Auth.ResetPassword;

public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
{
    public ResetPasswordCommand(ResetPasswordRequest payload)
    {
        Payload = payload;
    }

    public ResetPasswordRequest Payload { get; set; }
}