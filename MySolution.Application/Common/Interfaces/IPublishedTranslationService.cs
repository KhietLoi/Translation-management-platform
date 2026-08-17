namespace MySolution.Application.Common.Interfaces;

public interface IPublishedTranslationService
{
    Task <Dictionary<string, string>> GetTranslationsAsync (string blobFile, string languageCode,  CancellationToken cancellationToken);
}