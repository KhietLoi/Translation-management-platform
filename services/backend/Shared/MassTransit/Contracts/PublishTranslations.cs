using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface PublishTranslations : CorrelatedBy<Guid>
{
    PublishTranslationsEvent Content { get; }
}