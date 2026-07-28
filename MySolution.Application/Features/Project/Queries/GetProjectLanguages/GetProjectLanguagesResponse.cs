using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Queries.GetProjectLanguages;

public class GetProjectLanguagesResponse : BaseResponse <GetProjectLanguagesResult>
{
   
}
public class GetProjectLanguagesResult
{
    public List<GetProjectLanguageData> Languages { get; set; } = [];
}

public class GetProjectLanguageData
{
    public Guid LanguageId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}