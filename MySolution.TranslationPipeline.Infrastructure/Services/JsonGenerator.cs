using System.IO.Compression;
using System.Text.Json;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Models;

namespace MySolution.Infrastructure.ImportExport.Generators;

public class JsonGenerator : ITranslationGenerator
{
    public async Task<MemoryStream> GenerateAsync(IReadOnlyCollection<TranslationExportData> data, CancellationToken cancellationToken)
    {
        var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var languageData in data)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var entry = archive.CreateEntry(
                    $"{languageData.LanguageCode}.json",
                    CompressionLevel.Optimal);

                await using var entryStream = entry.Open();

                await JsonSerializer.SerializeAsync(
                    entryStream,
                    languageData.Translations,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    },
                    cancellationToken);
            }
        }

        zipStream.Position = 0;

        return zipStream;
    }
}