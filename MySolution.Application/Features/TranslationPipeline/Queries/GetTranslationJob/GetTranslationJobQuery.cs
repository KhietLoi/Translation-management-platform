using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJob;

public class GetTranslationJobQuery : IRequest<GetTranslationJobResponse>
{
    public Guid ProjectId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}