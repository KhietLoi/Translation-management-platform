using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJobDetail;

public class GetTranslationJobDetailQuery : IRequest<GetTranslationJobDetailResponse>
{
    public GetTranslationJobDetailRequest Payload { get; set; }

    public GetTranslationJobDetailQuery(GetTranslationJobDetailRequest payload)
    {
        Payload = payload;
    }
}