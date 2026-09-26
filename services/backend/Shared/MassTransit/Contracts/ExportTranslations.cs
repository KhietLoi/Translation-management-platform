using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface ExportTranslations : CorrelatedBy<Guid>
{
    ExportTranslationsEvent Content { get;  }
}