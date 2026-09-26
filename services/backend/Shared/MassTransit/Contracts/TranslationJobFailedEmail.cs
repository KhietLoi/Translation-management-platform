using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface TranslationJobFailedEmail : CorrelatedBy<Guid>
{
    TranslationJobFailedEmailEvent Content { get; }
}