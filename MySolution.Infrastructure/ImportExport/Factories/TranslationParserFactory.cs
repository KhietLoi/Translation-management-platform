using MySolution.Application.Common.Interfaces.File;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.ImportExport.Factories;

public class TranslationParserFactory : ITranslationParserFactory
{
    private readonly IEnumerable<ITranslationParser> _parsers;

    public TranslationParserFactory(IEnumerable<ITranslationParser> parsers)
    {
        _parsers = parsers;
    }

    public ITranslationParser GetParser(FileType format)
    {
        var parser = _parsers.FirstOrDefault(x => x.Format == format);
        if (parser == null)
        {
            throw new NotSupportedException($"Import format '{format}' is not supported.");
        }

        return parser;
    }
}