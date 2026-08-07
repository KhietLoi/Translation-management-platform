using MediatR;

namespace MySolution.Application.Features.ImportExport.Commands.ProcessImportTranslations;

public class ProcessImportTranslationsHandler : IRequestHandler<ProcessImportTranslationsCommand>
{
    public Task Handle(ProcessImportTranslationsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}