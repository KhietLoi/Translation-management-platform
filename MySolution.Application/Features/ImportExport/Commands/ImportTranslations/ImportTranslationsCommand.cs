using MediatR;

namespace MySolution.Application.Features.ImportExport.Commands.ImportTranslations;

public class ImportTranslationsCommand : IRequest<ImportTranslationsResponse>
{
    public ImportTranslationsRequest Payload { get; set; }

    public ImportTranslationsCommand(ImportTranslationsRequest payload)
    {
        Payload = payload;
    }
}