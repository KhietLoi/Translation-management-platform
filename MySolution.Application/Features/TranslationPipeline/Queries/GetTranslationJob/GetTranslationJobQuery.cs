using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJob;

public class GetTranslationJobQuery : IRequest<GetTranslationJobResponse>
{
    public GetTranslationJobRequest Payload { get; set; }

    public GetTranslationJobQuery(GetTranslationJobRequest payload)
    {
        Payload = payload;
    }
}