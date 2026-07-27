using System.Diagnostics.Contracts;

namespace MySolution.Application.Features.Language.Commands.UpdateLanguage;

public class UpdateLanguageRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
}