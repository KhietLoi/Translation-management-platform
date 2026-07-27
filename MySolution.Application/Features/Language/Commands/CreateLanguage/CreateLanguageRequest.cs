namespace MySolution.Application.Features.Language.Commands.CreateLanguage;

public class CreateLanguageRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
}