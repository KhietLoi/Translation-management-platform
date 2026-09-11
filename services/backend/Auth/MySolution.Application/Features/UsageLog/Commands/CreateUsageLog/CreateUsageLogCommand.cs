using MediatR;

namespace MySolution.Application.Features.UsageLog.Commands.CreateUsageLog;

public class CreateUsageLogCommand : IRequest
{
    public CreateUsageLogRequest Payload { get; set; }

    public CreateUsageLogCommand(CreateUsageLogRequest payload)
    {
        Payload = payload;
    }
}