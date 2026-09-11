using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Dashboard.Queries.GetDashboardValue;

public class GetDashboardValueResponse : BaseResponse <GetDashboardValueResult>
{

}

public class GetDashboardValueResult
{
    public DashboardSummaryDto DashboardSummary { get; set; } = new();
    public List<LanguageProgressDto> LanguageProgress { get; set; } = [];
    public List <RecentActivityDto> RecentActivities { get; set; } = [];
}

public class RecentActivityDto
{
 
    public Guid Id { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string? Message { get; set; }
    public Guid? ProjectId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LanguageProgressDto
{
    public Guid LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string LanguageName { get; set; } = string.Empty;
    public decimal Progress { get; set; }
}

public class DashboardSummaryDto
{
    public int TotalProjects { get; set; }
    public int TotalTranslationKeys { get; set; }
    public decimal TranslationProgress { get; set; }
    public int PendingReview { get; set; }
}