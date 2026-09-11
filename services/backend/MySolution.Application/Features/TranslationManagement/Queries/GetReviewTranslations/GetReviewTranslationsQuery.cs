using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetReviewTranslations;

public class GetReviewTranslationsQuery : IRequest<GetReviewTranslationsResponse>
{
    public GetReviewTranslationsRequest Payload { get; set; }

    public GetReviewTranslationsQuery(GetReviewTranslationsRequest payload)
    {
        Payload = payload;
    }
}