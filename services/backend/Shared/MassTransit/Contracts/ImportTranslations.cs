using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface ImportTranslations : CorrelatedBy<Guid>
{
    ImportTranslationsEvent Content { get; }
}