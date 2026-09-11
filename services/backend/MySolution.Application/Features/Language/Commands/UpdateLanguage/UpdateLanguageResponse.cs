using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Language.Commands.UpdateLanguage;

public class UpdateLanguageResponse : BaseResponse <UpdateLanguageData>
{
}
public class UpdateLanguageData
{
    public Guid LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 
}