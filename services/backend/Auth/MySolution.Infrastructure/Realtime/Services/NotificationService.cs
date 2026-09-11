using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Models.Realtime;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using MySolution.Infrastructure.Realtime.Hubs;

namespace MySolution.Infrastructure.Realtime.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<TranslationHub> _hub;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<TranslationHub> hub,
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger)
    {
        _hub = hub;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task NotifyUserAsync(
        Guid userId,
        Guid projectId,
        Guid? triggeredByUserId,
        string title,
        string message,
        NotificationType type,
        string? navigationUrl,
        NotificationReferenceType? notificationReferenceType,
        Guid? referenceId,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            ProjectId = projectId,
            TriggeredByUserId = triggeredByUserId ?? Guid.Empty,
            Title = title,
            Message = message,
            Type = type,
            NavigationUrl = navigationUrl,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notification.Add(notification);
        await _unitOfWork.SaveAsync(cancellationToken);

        string createdBy = string.Empty;
        if (triggeredByUserId.HasValue)
        {
            createdBy = await _unitOfWork.User.GetUserNameAsync(triggeredByUserId.Value);
        }

        var payload = new NotificationMessage
        {
            NotificationId = notification.Id,
            ProjectId = notification.ProjectId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            IsRead = notification.IsRead,
            NavigationUrl = notification.NavigationUrl,
            CreatedAt = notification.CreatedAt,
            CreatedBy = createdBy
        };

        await _hub.Clients.User(userId.ToString()).SendAsync("NotificationReceived", payload, cancellationToken);
    }
   

    public async Task NotifyProjectAsync
    (
        Guid projectId,
        Guid? triggeredByUserId,
        string title,
        string message,
        NotificationType type,
        string? navigationUrl,
        
        NotificationReferenceType? referenceType,
        Guid? referenceId,
        CancellationToken cancellationToken
    )
    {
        var members = await _unitOfWork.ProjectMember.GetByProjectIdAsync(projectId);
        if (!members.Any())
        {
            _logger.LogWarning("No members found for project {ProjectId}", projectId);
            return;
        }

        var createdAt = DateTime.UtcNow;
        var notifications = members
            .Select(member => new Notification
            {
                Id = Guid.CreateVersion7(),
                UserId = member.UserId,
                ProjectId = projectId,
                TriggeredByUserId = triggeredByUserId ?? Guid.Empty,
                Title = title,
                Message = message,
                Type = type,
                NavigationUrl = navigationUrl,
                IsRead = false,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                CreatedAt = createdAt
            }).ToList();

        await _unitOfWork.Notification.AddRange(notifications);
        await _unitOfWork.SaveAsync(cancellationToken);
        string createdBy = string.Empty;
        if (triggeredByUserId.HasValue)
        {
            createdBy = await _unitOfWork.User
                .GetUserNameAsync(triggeredByUserId.Value);
        }

        foreach (var notification in notifications)
        {
            var payload = new NotificationMessage
            {
                NotificationId = notification.Id,
                ProjectId = notification.ProjectId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                IsRead = notification.IsRead,
                NavigationUrl = notification.NavigationUrl,
                CreatedAt = notification.CreatedAt,
                CreatedBy = createdBy
            };

            await _hub.Clients
                .User(notification.UserId.ToString())
                .SendAsync("NotificationReceived", payload, cancellationToken);
        }
    }
    
}
