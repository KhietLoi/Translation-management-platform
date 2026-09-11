using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationKeys;

public class GetTranslationKeysQuery : IRequest<GetTranslationKeysResponse>
{
    public Guid? ProjectId { get; set; }
    public Guid? NamespaceId { get; set; }
    public string? Keyword { get; set; }
    
    public GetTranslationKeysQuery(Guid? projectId, Guid? namespaceId, string? keyword)
    {
        ProjectId = projectId;
        NamespaceId = namespaceId;
        Keyword = keyword;
    }
}