using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDetail;

public class GetReleaseDetailQuery : IRequest<GetReleaseDetailResponse>
{
    public GetReleaseDetailRequest Payload { get; set; }

    public GetReleaseDetailQuery(GetReleaseDetailRequest payload)
    {
        Payload = payload;
    }
}