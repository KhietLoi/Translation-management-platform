using MediatR;

namespace MySolution.Application.Features.User.Events;

public sealed record UserCreatedEvent(Guid UserId, string UserName, string Email) : INotification;