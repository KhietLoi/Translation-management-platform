using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;

public class PublishTranslationsCommand : IRequest<PublishTranslationsResponse>
{
    public PublishTranslationsRequest Payload { get; set; }

    public PublishTranslationsCommand(PublishTranslationsRequest payload)
    {
        Payload = payload;
    }
}