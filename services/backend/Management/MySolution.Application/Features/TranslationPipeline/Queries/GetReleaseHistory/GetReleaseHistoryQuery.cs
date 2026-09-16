using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseHistory;

public class GetReleaseHistoryQuery : IRequest<GetReleaseHistoryResponse>
{
    public Guid ProjectId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    public GetReleaseHistoryQuery(Guid projectId, int pageNumber = 1, int pageSize = 20)
    {
        ProjectId = projectId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
    
}