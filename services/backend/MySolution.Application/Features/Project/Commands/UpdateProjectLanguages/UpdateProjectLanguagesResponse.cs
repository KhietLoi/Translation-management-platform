using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectLanguages;

public class UpdateProjectLanguagesResponse : BaseResponse <UpdateProjectLanguagesResult>
{
}

public class UpdateProjectLanguagesResult
{
    public Guid ProjectId { get; set; }
    public List<UpdateProjectLanguagesData> Languages { get; set; } = [];
}

public class UpdateProjectLanguagesData
{
    public Guid LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}