using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.ReviewTranslation;

public class ReviewTranslationResponse
    : BaseResponse<ReviewTranslationData>
{
}

public class ReviewTranslationData
{
    public Guid Id { get; set; }
    public TranslationStatus Status { get; set; }
    public Guid ReviewerId { get; set; }
    public DateTime ReviewedAt { get; set; }
}