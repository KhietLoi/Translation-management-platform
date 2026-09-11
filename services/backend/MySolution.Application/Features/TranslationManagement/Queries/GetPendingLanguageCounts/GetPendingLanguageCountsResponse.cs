
using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetPendingLanguageCounts;

public class GetPendingLanguageCountsResponse : BaseResponse <GetPendingLanguageCountsResult>
{
}
public class GetPendingLanguageCountsResult
{
    public List<PendingLanguageCount> Languages { get; set; } = new();
}