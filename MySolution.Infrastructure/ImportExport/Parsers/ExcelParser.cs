using ClosedXML.Excel;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Parsers;

public class ExcelParser : ITranslationParser
{
    public FileType Format => FileType.Excel;
    public async Task<Dictionary<string, string>> ParseAsync(Stream stream, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string>();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        
        foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
        {
            cancellationToken.ThrowIfCancellationRequested();
            var key = row.Cell(1).GetString();
            var value = row.Cell(2).GetString();
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            result[key.Trim()] = value?.Trim() ?? string.Empty;
        }
        
        return result;
    }
}