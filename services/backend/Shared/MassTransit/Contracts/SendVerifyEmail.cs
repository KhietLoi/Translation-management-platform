using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface SendVerifyEmail : CorrelatedBy<Guid>
{
    SendVerifyEmailEvent Content { get;  }
}