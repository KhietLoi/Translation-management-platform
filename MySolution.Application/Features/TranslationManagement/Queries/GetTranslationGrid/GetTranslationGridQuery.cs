using MediatR;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationGrid;

public class GetTranslationGridQuery : IRequest<GetTranslationGridResponse>
{
    public Guid ProjectId { get; set; }

    public Guid? NamespaceId { get; set; }

    public string? Keyword { get; set; }

    public TranslationStatus? Status { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}