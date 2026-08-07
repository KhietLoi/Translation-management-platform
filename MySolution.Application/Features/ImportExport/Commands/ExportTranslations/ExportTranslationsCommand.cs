using MediatR;

namespace MySolution.Application.Features.ImportExport.Commands.ExportTranslations;

public class ExportTranslationsCommand : IRequest<ExportTranslationsResponse>
{
    public ExportTranslationsRequest Payload { get; set; }

    public ExportTranslationsCommand(ExportTranslationsRequest payload)
    {
        Payload = payload;
    }
}