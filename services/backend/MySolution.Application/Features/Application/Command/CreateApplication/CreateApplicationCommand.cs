using MediatR;

namespace MySolution.Application.Features.Application.Command.CreateApplication;

public class CreateApplicationCommand : IRequest<CreateApplicationResponse>
{
    public CreateApplicationRequest Payload { get; set; }

    public CreateApplicationCommand(CreateApplicationRequest payload)
    {
        Payload = payload;
    }
}