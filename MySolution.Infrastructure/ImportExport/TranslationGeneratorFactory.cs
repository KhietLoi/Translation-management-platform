using MySolution.Application.Common.Interfaces.File;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport;

public class TranslationGeneratorFactory : ITranslationGeneratorFactory
{
    private readonly IEnumerable<ITranslationGenerator> _generators;
    public TranslationGeneratorFactory(IEnumerable<ITranslationGenerator> generators)
    {
        _generators = generators;
    }
    public ITranslationGenerator GetGenerator(
        ExportFileType format)
    {
        var generator =
            _generators.FirstOrDefault(
                x => x.Format == format);

        if (generator == null)
        {
            throw new NotSupportedException(
                $"Export format '{format}' is not supported.");
        }

        return generator;
    }
}