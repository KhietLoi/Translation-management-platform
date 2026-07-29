using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeyById;

public class GetTranslationKeyByIdResponse : BaseResponse <GetTranslationKeyByIdData>
{

}

public class GetTranslationKeyByIdData
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid NamespaceId { get; set; }
    public string NamespaceName { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}