using MediatR;

namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordCommand : IRequest<ChangePasswordResponse>
{
    public ChangePasswordRequest Payload { get; set; }
    public ChangePasswordCommand(ChangePasswordRequest payload)
    {
        Payload = payload;
    }
}