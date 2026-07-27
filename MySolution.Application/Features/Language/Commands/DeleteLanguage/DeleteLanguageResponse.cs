using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Language.Commands.DeleteLanguage;

public class DeleteLanguageResponse : BaseResponse <DeleteLanguageData>
{
}
public class DeleteLanguageData
{
    public Guid LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
}