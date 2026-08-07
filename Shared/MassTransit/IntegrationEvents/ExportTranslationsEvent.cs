
using Shared.Enums;

namespace Shared.MassTransit.IntegrationEvents;

public class ExportTranslationsEvent
{
   public Guid JobId { get; set; }
}