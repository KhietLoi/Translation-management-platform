using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessImportTranslations;

public class ProcessImportTranslationsCommand : IRequest
{
    public Guid JobId { get; set; }
    
    public ProcessImportTranslationsCommand(Guid jobId)
    {
        JobId = jobId;
    }
}