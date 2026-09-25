using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface TranslationJobCompletedEmail : CorrelatedBy<Guid>
{
    TranslationJobCompletedEmailEvent Content { get; }
}