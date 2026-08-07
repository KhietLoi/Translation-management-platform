using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.File;

public interface ITranslationGeneratorFactory
{
    ITranslationGenerator GetGenerator(ExportFileType format);
}