using MediatR;

namespace MySolution.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public ForgotPasswordCommand(ForgotPasswordRequest payload)
    {
        Payload = payload;
    }

    public ForgotPasswordRequest Payload { get; set; }
}