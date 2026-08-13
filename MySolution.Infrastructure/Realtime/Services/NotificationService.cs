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
        string title,
        string message,
        NotificationType type,
        string? navigationUrl,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            NavigationUrl = navigationUrl,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.Notification.Add(notification);
        await _unitOfWork.SaveAsync(cancellationToken);

        var payload = new NotificationMessage
        {
            Title = notification.Title,
            Message = notification.Message,
            Type = type.ToString(),
            IsRead = notification.IsRead,
            NavigationUrl = notification.NavigationUrl,
            CreatedAt = notification.CreatedAt
        };

        await _hub.Clients
            .User(userId.ToString())
            .SendAsync("NotificationReceived", payload, cancellationToken);
    }

    public async Task NotifyProjectAsync(
        Guid projectId,
        string title,
        string message,
        string createdBy,
        NotificationType type,
        string? navigationUrl,
        CancellationToken cancellationToken)
    {
        var members = await _unitOfWork.ProjectMember
            .GetByProjectIdAsync(projectId);
        
        _logger.LogInformation($"Members for project {projectId}: {members}");
        var notifications = members
            .Select(member => new Notification
            {
                UserId = member.UserId,
                ProjectId = projectId,
                Title = title,
                Message = message,
                Type = type,
                NavigationUrl = navigationUrl,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        await _unitOfWork.Notification.AddRange(notifications);
        await _unitOfWork.SaveAsync(cancellationToken);

        var payload = new NotificationMessage
        {
            Title = title,
            Message = message,
            CreatedBy = createdBy,
            IsRead = false,
            Type =  type.ToString(),
            NavigationUrl = navigationUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _hub.Clients
            .Group(projectId.ToString())
            .SendAsync("NotificationReceived", payload, cancellationToken);
    }
}