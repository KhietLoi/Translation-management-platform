namespace MySolution.Application.Common.Models;

public class TranslationImportData
{
    public string LanguageCode { get; set; } =  string.Empty;
    public Dictionary<string, string> Translations { get; set; } = new();
}