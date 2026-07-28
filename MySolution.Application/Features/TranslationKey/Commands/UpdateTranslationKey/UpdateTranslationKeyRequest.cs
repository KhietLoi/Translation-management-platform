namespace MySolution.Application.Features.TranslationKey.Commands.UpdateTranslationKey;

public class UpdateTranslationKeyRequest
{
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    
}