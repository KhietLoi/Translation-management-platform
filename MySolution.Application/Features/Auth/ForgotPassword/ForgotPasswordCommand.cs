using MediatR;

namespace MySolution.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public ForgotPasswordRequest Payload { get; set; }

    public ForgotPasswordCommand(ForgotPasswordRequest payload)
    {
        Payload = payload;
    }
}