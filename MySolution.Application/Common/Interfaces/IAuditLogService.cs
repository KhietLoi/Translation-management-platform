using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task CreateAsync(
        Guid userId,
        AuditAction action,
        string entityName,
        Guid entityId,
        object? oldValue,
        object? newValue);
}