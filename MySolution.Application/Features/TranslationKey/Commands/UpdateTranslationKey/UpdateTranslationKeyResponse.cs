using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationKey.Commands.UpdateTranslationKey;

public class UpdateTranslationKeyResponse : BaseResponse <UpdateTranslationKeyData>
{

}

public class UpdateTranslationKeyData
{
    public Guid Id { get; set; }
    public Guid NamespaceId { get; set; }
    public Guid ProjectId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}