using System.Text;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Parsers;

public class CsvParser : ITranslationParser
{
    public FileType Format => FileType.Csv;

    public async Task<Dictionary<string, string>> ParseAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string>();
        using var reader = new StreamReader(stream);

        // Skip header
        await reader.ReadLineAsync(cancellationToken);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = ParseCsvLine(line);

            if (values.Count < 2)
            {
                continue;
            }

            var key = values[0].Trim();
            var value = values[1].Trim();

            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            result[key] = value;
        }

        return result;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var current = new StringBuilder();
        var insideQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var character = line[i];

            if (character == '"')
            {
                // Escaped quote: ""
                if (insideQuotes &&
                    i + 1 < line.Length &&
                    line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                insideQuotes = !insideQuotes;
                continue;
            }

            // Comma outside quotes = next column
            if (character == ',' && !insideQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(character);
        }

        // Add last column
        values.Add(current.ToString());

        return values;
    }
}
