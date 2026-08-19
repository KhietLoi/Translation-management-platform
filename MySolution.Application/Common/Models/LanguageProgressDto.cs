namespace MySolution.Application.Common.Models;

public class LanguageProgressDto
{
    public Guid LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Translated { get; set; }
}