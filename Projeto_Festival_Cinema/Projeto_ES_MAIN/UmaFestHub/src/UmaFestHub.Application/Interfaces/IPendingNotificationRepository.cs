// In-app notifications: EF-backed queue (enqueue by role/user, list pending, acknowledge).
using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

/// <summary>Persisted notification queue (one row per user; role targets are fan-out at enqueue).</summary>
public interface IPendingNotificationRepository
{
	/// <summary>Creates one undelivered row per user with <paramref name="role"/>, skipping duplicate (same user + correlation) still pending.</summary>
	Task<IReadOnlyList<(Guid UserId, Guid NotificationId)>> EnqueueForRoleAsync(
		string role,
		NotificationTemplate template,
		string? correlationId,
		CancellationToken cancellationToken = default);

	/// <returns><see langword="true"/> when a new row was inserted; <see langword="false"/> when skipped (duplicate user + correlation still pending).</returns>
	Task<bool> EnqueueForUserAsync(Guid id, Guid userId, NotificationTemplate template, string? correlationId, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<PendingNotificationItemDto>> GetUndeliveredForUserAsync(Guid userId, CancellationToken cancellationToken = default);

	Task AcknowledgeAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
}
