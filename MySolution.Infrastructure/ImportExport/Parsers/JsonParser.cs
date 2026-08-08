using System.IO.Compression;
using System.Text.Json;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Parsers;

public class JsonParser : ITranslationParser
{
    public FileType Format => FileType.Json;
    public async Task<Dictionary<string, string>> ParseAsync(Stream stream, CancellationToken cancellationToken)
    {
        return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>
            (stream, cancellationToken: cancellationToken) ?? new Dictionary<string, string>();
    }
}