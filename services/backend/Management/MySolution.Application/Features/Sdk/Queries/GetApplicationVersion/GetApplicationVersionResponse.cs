using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationVersion;

public class GetApplicationVersionResponse : BaseResponse <GetApplicationVersionResponseData>
{
}

public class GetApplicationVersionResponseData
{
    public Guid ProjectId { get; set; }
    public int Version { get; set; }
    public DateTime PublishedAt { get; set; }
}