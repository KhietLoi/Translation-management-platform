using System.IO.Compression;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Generators;

public class CsvGenerator : ITranslationGenerator
{
    public ExportFileType Format => ExportFileType.Csv;

    public async Task<MemoryStream> GenerateAsync(IReadOnlyCollection<TranslationExportData> data, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();

        using var archive =
            new ZipArchive(stream, ZipArchiveMode.Create, true);

        foreach (var language in data)
        {
            var entry = archive.CreateEntry($"{language.LanguageCode}.csv");
            await using var entryStream = entry.Open();
            using var writer = new StreamWriter(entryStream);
            await writer.WriteLineAsync("Key,Value");

            foreach (var item in language.Translations)
            {
                await writer.WriteLineAsync($"\"{item.Key}\",\"{item.Value}\"");
            }

            await writer.FlushAsync();
        }

        stream.Position = 0;

        return stream;
    }
}