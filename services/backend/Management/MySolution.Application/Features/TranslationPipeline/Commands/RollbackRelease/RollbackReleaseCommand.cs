using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Commands.RollbackRelease;

public class RollbackReleaseCommand : IRequest<RollbackReleaseResponse>
{
   public Guid ReleaseId { get; set; }

   public RollbackReleaseCommand(Guid releaseId)
   {
      ReleaseId = releaseId;
   }
}