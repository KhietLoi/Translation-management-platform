using MediatR;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationVersion;

public class GetApplicationVersionQuery : IRequest<GetApplicationVersionResponse>
{
   public Guid ProjectId { get; set; }
}