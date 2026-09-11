using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.File;

public interface ITranslationParserFactory
{
    ITranslationParser GetParser(FileType format);
}