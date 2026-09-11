using System.IO.Compression;
using System.Text;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Generators;

public class CsvGenerator : ITranslationGenerator
{
    public FileType Format => FileType.Csv;

    public async Task<MemoryStream> GenerateAsync(
        IReadOnlyCollection<TranslationExportData> data,
        CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();

        using (var archive = new ZipArchive(
                   stream,
                   ZipArchiveMode.Create,
                   leaveOpen: true))
        {
            foreach (var language in data)
            {
                var entry = archive.CreateEntry(
                    $"{language.LanguageCode}.csv",
                    CompressionLevel.Optimal);

                await using var entryStream = entry.Open();

                using var writer = new StreamWriter(
                    entryStream,
                    new UTF8Encoding(true));

                await writer.WriteLineAsync("Key,Value");

                foreach (var item in language.Translations)
                {
                    var key = item.Key.Replace("\"", "\"\"");
                    var value = item.Value.Replace("\"", "\"\"");

                    await writer.WriteLineAsync(
                        $"\"{key}\",\"{value}\"");
                }

                await writer.FlushAsync();
            }
        } // <-- archive.Dispose() chạy ở đây

        stream.Position = 0;

        return stream;
    }
}