namespace MySolution.TranslationPipeline.Application.Common.Models;

public class ExportTranslationResult
{
    public string FileName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = null!;
}