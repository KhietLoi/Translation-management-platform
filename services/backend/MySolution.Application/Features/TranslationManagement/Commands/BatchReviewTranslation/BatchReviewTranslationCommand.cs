using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchReviewTranslation;

public class BatchReviewTranslationCommand : IRequest<BatchReviewTranslationResponse>
{
    public BatchReviewTranslationRequest Payload { get; set; }

    public BatchReviewTranslationCommand(BatchReviewTranslationRequest payload)
    {
        Payload = payload;
    }
}