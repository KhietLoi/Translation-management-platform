using ClosedXML.Excel;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Generators;

public class ExcelGenerator : ITranslationGenerator
{
    public ExportFileType Format => ExportFileType.Excel;

    public async Task<MemoryStream> GenerateAsync(IReadOnlyCollection<TranslationExportData> data, CancellationToken cancellationToken)
    {
        var workbook = new XLWorkbook();
        
        foreach (var language in data)
        {
            var worksheet = workbook.Worksheets.Add(language.LanguageCode);

            worksheet.Cell(1, 1).Value = "Key";
            worksheet.Cell(1, 2).Value = "Value";
            var row = 2;

            foreach (var item in language.Translations)
            {
                worksheet.Cell(row, 1).Value = item.Key;
                worksheet.Cell(row, 2).Value = item.Value;
                row++;
            }
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return await Task.FromResult(stream);
    }
}