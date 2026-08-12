using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationPipeline.Commands.RollbackRelease;

public class RollbackReleaseResponse : BaseResponse <RollbackReleaseData>
{
}

public class RollbackReleaseData
{
    public Guid ReleaseId { get; set; }
    public Guid ProjectId { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
}