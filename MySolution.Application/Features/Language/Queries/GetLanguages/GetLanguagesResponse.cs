using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Language.Queries.GetLanguages;

public class GetLanguagesResponse : BaseResponse <GetLanguageResult>
{
}

public class GetLanguageResult
{
    public List <GetLanguageData>?  Languages { get; set; }
}

public class GetLanguageData
{
    public Guid LanguageId { get; set; }
    public string Code { get; set; } = String.Empty;
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; }
}