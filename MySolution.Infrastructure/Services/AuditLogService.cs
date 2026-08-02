using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(IUnitOfWork unitOfWork, ILogger<AuditLogService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    

    public async Task CreateAsync(Guid userId, AuditAction action, string entityName, Guid entityId, object? oldValue, object? newValue)
    {
        var auditLog = new Domain.Entities.AuditLog
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValue = oldValue != null ? System.Text.Json.JsonSerializer.Serialize(oldValue) : null,
            NewValue = newValue != null ? System.Text.Json.JsonSerializer.Serialize(newValue) : null,
            CreatedAt = DateTime.UtcNow
        };  
        _logger.LogInformation("Creating audit log entry. Action: {Action}, Entity: {EntityName}, EntityId: {EntityId}, UserId: {UserId}", action, entityName, entityId, userId);
        await _unitOfWork.AuditLog.Add(auditLog);
    }
}