namespace MySolution.TranslationPipeline.Application.Common.Models;

public class TranslationExportData
{
    public string LanguageCode { get; set; } = null!;
    public Dictionary<string, string> Translations { get; set; } = new();
    
}