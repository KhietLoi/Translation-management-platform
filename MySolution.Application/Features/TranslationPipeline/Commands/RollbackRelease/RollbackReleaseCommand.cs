using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Commands.RollbackRelease;

public class RollbackReleaseCommand : IRequest<RollbackReleaseResponse>
{
    public RollbackReleaseRequest Payload { get; set; }

    public RollbackReleaseCommand(RollbackReleaseRequest payload)
    {
        Payload = payload;
    }
}