using MediatR;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationPackage;

public class GetApplicationPackageQuery : IRequest<GetApplicationPackageResponse>
{
    public Guid ProjectId { get; set; }
}