using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDiff;

public class GetReleaseDiffQuery : IRequest<GetReleaseDiffResponse>
{
    public Guid TargetReleaseId { get; set; }

    public GetReleaseDiffQuery(Guid targetReleaseId)
    {
        TargetReleaseId = targetReleaseId;
    }
}