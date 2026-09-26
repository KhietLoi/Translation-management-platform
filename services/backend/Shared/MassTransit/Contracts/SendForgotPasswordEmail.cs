using MassTransit;
using Shared.MassTransit.IntegrationEvents;

namespace Shared.MassTransit.Contracts;

[ConfigureConsumeTopology(false)]
public interface SendForgotPasswordEmail : CorrelatedBy<Guid>
{
    SendForgotPasswordEmailEvent Content { get;  }
}