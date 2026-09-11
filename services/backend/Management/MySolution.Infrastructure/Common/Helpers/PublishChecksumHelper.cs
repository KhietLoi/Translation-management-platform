using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MySolution.Application.Common.Models;

namespace MySolution.Infrastructure.Common.Helpers;

public static class PublishChecksumHelper
{
    public static string Calculate(
        IReadOnlyCollection<TranslationExportData> translations)
    {
        var content = translations
            .OrderBy(x => x.LanguageCode)
            .SelectMany(x =>
                x.Translations
                    .OrderBy(t => t.Key)
                    .Select(t => new
                    {
                        Language = x.LanguageCode,
                        Key = t.Key,
                        Value = t.Value
                    }))
            .ToList();

        var json = JsonSerializer.Serialize(content);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(json)));
    }
}