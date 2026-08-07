using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;


namespace MySolution.Application.Common.Interfaces.File;

public interface ITranslationGenerator
{
    FileType Format { get; }
    Task<MemoryStream> GenerateAsync(IReadOnlyCollection<TranslationExportData> data, CancellationToken cancellationToken);
}