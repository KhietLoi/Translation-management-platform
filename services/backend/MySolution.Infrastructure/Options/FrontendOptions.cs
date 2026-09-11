using System.ComponentModel.DataAnnotations;

namespace MySolution.Infrastructure.Options;

public class FrontendOptions
{
    public const string SectionName = "Frontend";

    [Required] public string BaseUrl { get; set; } = string.Empty;
}