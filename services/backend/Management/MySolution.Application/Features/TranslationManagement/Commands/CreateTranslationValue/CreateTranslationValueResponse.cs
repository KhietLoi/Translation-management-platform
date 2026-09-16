using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationValue;

public class CreateTranslationValueResponse : BaseResponse <CreateTranslationValueData>
{
}

public class CreateTranslationValueData
{
    public Guid Id { get; set; }
    public Guid TranslationKeyId { get; set; }
    public Guid LanguageId { get; set; }
    public string Value { get; set; } = string.Empty;
    public TranslationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}