using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationValue.Commands.CreateTranslationValue;

public class CreateTranslationValueRequest
{
    public Guid TranslationKeyId { get; set; }
    public Guid LanguageId { get; set; }
    public string Value { get; set; } = string.Empty;
}