using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Language.Commands.CreateLanguage;

public class CreateLanguageResponse : BaseResponse <CreateLanguageData>
{
    
}

public class CreateLanguageData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
}