using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task CreateAsync(
        Guid userId,
        AuditAction action,
        string entityName,
        Guid entityId,
        Guid? projectId,
        object? oldValue,
        object? newValue);
}