namespace MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;

public class PublishTranslationsRequest
{
    public Guid ProjectId { get; set; }
    public string? Notes { get; set; }
}