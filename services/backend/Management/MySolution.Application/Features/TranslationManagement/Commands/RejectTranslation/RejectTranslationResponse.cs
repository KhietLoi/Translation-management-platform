using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.RejectTranslation;

public class RejectTranslationResponse : BaseResponse <RejectTranslationData>
{

}

public class RejectTranslationData
{
    public Guid Id { get; set; }
    public TranslationStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime UpdatedAt { get; set; }
}