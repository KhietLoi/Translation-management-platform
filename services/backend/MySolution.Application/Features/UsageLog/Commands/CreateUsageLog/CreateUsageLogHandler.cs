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

    public async Task Handle(
        CreateUsageLogCommand request,
        CancellationToken cancellationToken)
    {
        var functionName = nameof(CreateUsageLogHandler);

        _logger.LogInformation(
            "{FunctionName} => Handling CreateUsageLogCommand",
            functionName);

        try
        {
            var payload = request.Payload;

            var usageLog = new ApiKeyUsageLog
            {
                Id = Guid.CreateVersion7(),
                ApiKeyId = payload.ApiKeyId,
                ApplicationId = payload.ApplicationId,
                Endpoint = payload.Endpoint,
                Method = payload.Method,
                StatusCode = payload.StatusCode,
                DurationMs = payload.DurationMs,
                IpAddress = payload.IpAddress,
                UserAgent = payload.UserAgent,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ApiKeyUsageLog.Add(usageLog);

            _logger.LogInformation(
                "{FunctionName} => Before SaveAsync",
                functionName);

            await _unitOfWork.SaveAsync(cancellationToken);

            _logger.LogInformation(
                "{FunctionName} => After SaveAsync",
                functionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} => Failed to create usage log", functionName);
            throw;
        }
    }
    

    #endregion
}