using System.ComponentModel.DataAnnotations;

namespace MySolution.Email.Infrastructure.Options;

public sealed class SendGridOptions
{
    public const string SectionName = "SendGrid";
    [Required] public string ApiKey { get; set; } = string.Empty;
    [Required] public string FromEmail { get; set; } = string.Empty;
    [Required] public string FromName { get; set; } = string.Empty;
}