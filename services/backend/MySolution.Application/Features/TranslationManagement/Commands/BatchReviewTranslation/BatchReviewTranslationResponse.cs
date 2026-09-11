

using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchReviewTranslation;

public class BatchReviewTranslationResponse : BaseResponse <BatchReviewTranslationData>
{

}   

public class BatchReviewTranslationData
{
    public int Total { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
}   