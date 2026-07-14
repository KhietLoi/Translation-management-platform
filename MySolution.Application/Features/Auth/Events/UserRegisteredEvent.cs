using MediatR;

namespace MySolution.Application.Features.Auth.Events;

public sealed record UserRegisteredEvent(Guid UserId, string Username, string Email) : INotification;