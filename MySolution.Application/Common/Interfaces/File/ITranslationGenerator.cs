using MySolution.Application.Common.Models;


namespace MySolution.Application.Common.Interfaces.File;

public interface ITranslationGenerator
{
    Task<MemoryStream> GenerateAsync(IReadOnlyCollection<TranslationExportData> data, CancellationToken cancellationToken);
}