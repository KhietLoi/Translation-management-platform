using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDiff;

public class GetReleaseDiffQuery : IRequest<GetReleaseDiffResponse>
{
    //public Guid SourceReleaseId { get; set; }
    public Guid TargetReleaseId { get; set; }

    public GetReleaseDiffQuery(Guid targetReleaseId)
    {
       // SourceReleaseId = sourceReleaseId;
        TargetReleaseId = targetReleaseId;
    }
}