namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationKey;

public class CreateTranslationKeyRequest
{
    public Guid ProjectId { get; set; }
    public Guid NamespaceId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
}