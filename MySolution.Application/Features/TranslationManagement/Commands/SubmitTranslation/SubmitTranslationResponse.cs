using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.SubmitTranslation;

public class SubmitTranslationResponse : BaseResponse <SubmitTranslationData>
{

}

public class SubmitTranslationData
{
    public Guid Id { get; set; }
    public Guid LanguageId { get; set; }
    public string? Value { get; set; }
    public TranslationStatus Status { get; set; }
    public Guid? TranslatedBy { get; set; }
    public DateTime? TranslatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}