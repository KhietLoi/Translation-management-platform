using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.File;

public interface ITranslationParser
{
    FileType Format { get; }
    Task<Dictionary<string, string>> ParseAsync(Stream stream, CancellationToken cancellationToken);
}