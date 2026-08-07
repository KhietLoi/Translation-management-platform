using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class ImportTranslationsConsumer : IConsumer<ImportTranslationsEvent>
{
    public Task Consume(ConsumeContext<ImportTranslationsEvent> context)
    {
        throw new NotImplementedException();
    }
}