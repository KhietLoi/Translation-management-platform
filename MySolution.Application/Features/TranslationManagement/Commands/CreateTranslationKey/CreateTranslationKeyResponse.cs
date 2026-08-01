using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationKey;

public class CreateTranslationKeyResponse : BaseResponse <CreateTranslationKeyData>
{

}

public class CreateTranslationKeyData
{
    public Guid Id { get; set; }
    public Guid NamespaceId { get; set; }
    public Guid ProjectId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}