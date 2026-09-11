using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchUpdateTranslation;

public class BatchUpdateTranslationCommand : IRequest<BatchUpdateTranslationResponse>
{
    public BatchUpdateTranslationRequest Payload { get; set; }

    public BatchUpdateTranslationCommand(BatchUpdateTranslationRequest payload)
    {
        Payload = payload;
    }
}