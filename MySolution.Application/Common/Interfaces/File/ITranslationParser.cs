using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.File;

public interface ITranslationParser
{
    FileType Format { get; }
    Task<IReadOnlyCollection<TranslationImportData>> ParseAsync(Stream stream, CancellationToken cancellationToken);
}