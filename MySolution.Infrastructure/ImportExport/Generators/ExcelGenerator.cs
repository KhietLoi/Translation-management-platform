using System.IO.Compression;
using ClosedXML.Excel;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Generators;

public class ExcelGenerator : ITranslationGenerator
{
    public FileType Format => FileType.Excel;

    public async Task<MemoryStream> GenerateAsync(
        IReadOnlyCollection<TranslationExportData> data,
        CancellationToken cancellationToken)
    {
        var zipStream = new MemoryStream();

        using (var archive = new ZipArchive(
                   zipStream,
                   ZipArchiveMode.Create,
                   leaveOpen: true))
        {
            foreach (var language in data)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var entry = archive.CreateEntry(
                    $"{language.LanguageCode}.xlsx",
                    CompressionLevel.Optimal);

                await using var entryStream = entry.Open();

                using var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add("Translations");

                worksheet.Cell(1, 1).Value = "Key";
                worksheet.Cell(1, 2).Value = "Value";

                worksheet.Row(1).Style.Font.Bold = true;

                var row = 2;

                foreach (var item in language.Translations)
                {
                    worksheet.Cell(row, 1).Value = item.Key;
                    worksheet.Cell(row, 2).Value = item.Value;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(entryStream);
            }
        }

        zipStream.Position = 0;

        return zipStream;
    }
}