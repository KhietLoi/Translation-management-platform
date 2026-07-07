using MediatR;

namespace MySolution.Application.Features.Auth.Register;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public RegisterRequest Payload { get; }
    public RegisterCommand(RegisterRequest payload)
    {
        Payload = payload;
    }
    
    
}