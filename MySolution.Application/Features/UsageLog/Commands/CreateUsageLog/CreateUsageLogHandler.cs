using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.UsageLog.Commands.CreateUsageLog;

public class CreateUsageLogHandler : IRequestHandler<CreateUsageLogCommand>
{
    private readonly ILogger<CreateUsageLogHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateUsageLogHandler
    (
        ILogger<CreateUsageLogHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateUsageLogCommand, CreateUsageLogResponse>

    public async Task Handle(CreateUsageLogCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateUsageLogHandler)} =>";
        _logger.LogInformation("{functionName} Handling CreateUsageLogCommand: ", functionName);

        await _unitOfWork.ApiKeyUsageLog.Add(
            new ApiKeyUsageLog
            {
               Id = Guid.CreateVersion7(),
               ApiKeyId = request.Payload.ApiKeyId,
               ApplicationId = request.Payload.ApplicationId,
               Endpoint = request.Payload.Endpoint,
               Method = request.Payload.Method,
               StatusCode = request.Payload.StatusCode,
               DurationMs = request.Payload.DurationMs,
               IpAddress = request.Payload.IpAddress,
               UserAgent = request.Payload.UserAgent,
               CreatedAt = DateTime.UtcNow
            }
        );
        
        await _unitOfWork.SaveAsync(cancellationToken);
    }

    #endregion
}