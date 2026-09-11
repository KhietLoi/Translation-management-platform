using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessPublishTranslations;

public class ProcessPublishTranslationsCommand : IRequest
{
   public Guid JobId { get; set; }
   
   public ProcessPublishTranslationsCommand(Guid jobId)
   {
      JobId = jobId;
   }
}  