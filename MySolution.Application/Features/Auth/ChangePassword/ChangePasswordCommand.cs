using MediatR;

namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordCommand : IRequest<ChangePasswordResponse>
{
    public ChangePasswordCommand(ChangePasswordRequest payload)
    {
        Payload = payload;
    }

    public ChangePasswordRequest Payload { get; set; }
}