using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Application.Queries.GetApplications;

public class GetApplicationsResponse : BaseResponse <GetApplicationsResult>
{

}

public class GetApplicationsResult
{
    public List <GetApplicationData>?  Applications { get; set; }
}

public class GetApplicationData
{
    public Guid ApplicationId { get; set; }
    public Guid ProjectId { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
}
