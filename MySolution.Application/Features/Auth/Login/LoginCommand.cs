using MediatR;

namespace MySolution.Application.Features.Auth.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public LoginRequest Payload { get; }
    public LoginCommand(LoginRequest payload)
    {
        Payload = payload;
    }
}