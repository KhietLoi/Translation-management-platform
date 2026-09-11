namespace MySolution.Email.Application.Common.Models;

public class StandardEmailTemplateData
{
    public string UserName { get; init; } = default!;
    public string ActionUrl { get; init; } = default!;
    public int ExpiryMinutes { get; init; }
}