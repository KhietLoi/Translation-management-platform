
using Shared.Enums;

namespace Shared.MassTransit.IntegrationEvents;

public class ExportTranslationsEvent
{
    public Guid ProjectId { get; set; }
    public ExportFileType Format  { get; set; }
}