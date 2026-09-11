namespace MySolution.Email.Application.Common.Models;

public class TranslationJobFailedEmailModel
{
    public string UserName { get; init; } = default!;
    public string JobType { get; init; } = default!;
    public string FileName { get; init; } = default!;
    public string ErrorMessage { get; init; } = default!;
    public string LogoUrl { get; init; } = default!;
    public int Year { get; init; }
}