using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

public interface INotificationService
{
	Task NotifyRoleAsync(string role, NotificationTemplate template, string? correlationId = null, CancellationToken cancellationToken = default);

	Task NotifyUserAsync(Guid userId, NotificationTemplate template, string? correlationId = null, CancellationToken cancellationToken = default);
}
