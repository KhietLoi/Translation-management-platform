using MediatR;

namespace MySolution.Application.Features.Application.Command.UpdateApplication;

public class UpdateApplicationCommand : IRequest<UpdateApplicationResponse>
{
    public UpdateApplicationRequest Payload { get; set; }
    public Guid ApplicationId { get; set; }

    public UpdateApplicationCommand(UpdateApplicationRequest payload , Guid applicationId)
    {
        Payload = payload;
        ApplicationId = applicationId;
    }
}