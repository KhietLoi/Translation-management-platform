using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface SendSetUpPasswordEmail : CorrelatedBy<Guid>
{
    SendSetUpPasswordEmailEvent Content { get; }
}