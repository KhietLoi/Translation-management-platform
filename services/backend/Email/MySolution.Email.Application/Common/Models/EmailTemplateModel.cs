namespace MySolution.Email.Application.Common.Models;

public class EmailTemplateModel
{
    public string UserName { get; init; } = default!;
    public string ActionUrl { get; init; } = default!;
    public int ExpiryMinutes { get; init; }
    public string LogoUrl { get; init; } = default!;
    public int Year { get; init; }
}