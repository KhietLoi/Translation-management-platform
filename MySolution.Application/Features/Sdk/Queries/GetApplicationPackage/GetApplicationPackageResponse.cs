using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationPackage;

public class GetApplicationPackageResponse : BaseResponse <GetApplicationPackageData>
{

}

public class GetApplicationPackageData
{
    public Guid ProjectId { get; set; }
    public int Version { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
}