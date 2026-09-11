using TranslationExportData = MySolution.TranslationPipeline.Application.Common.Models.TranslationExportData;

namespace MySolution.TranslationPipeline.Application.Common.Interfaces.File;

public interface ITranslationGenerator
{
    Task<MemoryStream> GenerateAsync(IReadOnlyCollection<TranslationExportData> data, CancellationToken cancellationToken);
}