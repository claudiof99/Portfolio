// In-app notifications: IPendingNotificationRepository — role fan-out, dedupe by correlation, ack rows.
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public sealed class PendingNotificationRepository : IPendingNotificationRepository
{
	private readonly AppDbContext _db;
	private readonly IUserRepository _users;

	public PendingNotificationRepository(AppDbContext db, IUserRepository users)
	{
		_db = db;
		_users = users;
	}

	public async Task<IReadOnlyList<(Guid UserId, Guid NotificationId)>> EnqueueForRoleAsync(
		string role,
		NotificationTemplate template,
		string? correlationId,
		CancellationToken cancellationToken = default)
	{
		if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var roleEnum))
		{
			return Array.Empty<(Guid, Guid)>();
		}

		var recipientIds = await _users.GetIdsHavingRoleAsync(roleEnum, cancellationToken);
		if (recipientIds.Count == 0)
		{
			return Array.Empty<(Guid, Guid)>();
		}

		var templateJson = NotificationTemplateJson.Serialize(template);
		var results = new List<(Guid UserId, Guid NotificationId)>();
		foreach (var userId in recipientIds)
		{
			if (userId == Guid.Empty)
			{
				continue;
			}

			if (!string.IsNullOrEmpty(correlationId))
			{
				var dup = await _db.Notifications.AnyAsync(
					q => q.TargetUserId == userId
						&& q.CorrelationId == correlationId
						&& q.AcknowledgedUtc == null,
					cancellationToken);
				if (dup)
				{
					continue;
				}
			}

			var id = Guid.NewGuid();
			var row = new Notification
			{
				Id = id,
				CreatedUtc = DateTime.UtcNow,
				Title = string.Empty,
				Message = string.Empty,
				TemplateJson = templateJson,
				CorrelationId = correlationId,
				TargetUserId = userId,
				TargetUserRole = roleEnum.ToString(),
				AcknowledgedUtc = null
			};
			await _db.Notifications.AddAsync(row, cancellationToken);
			results.Add((userId, id));
		}

		if (results.Count > 0)
		{
			await _db.SaveChangesAsync(cancellationToken);
		}

		return results;
	}

	/// <summary>Single label for DB clarity: highest-privilege role on the user account.</summary>
	private static string ResolveTargetUserRoleLabel(IEnumerable<UserRole> roles)
	{
		if (roles.Contains(UserRole.Admin))
		{
			return nameof(UserRole.Admin);
		}

		if (roles.Contains(UserRole.Manager))
		{
			return nameof(UserRole.Manager);
		}

		if (roles.Contains(UserRole.Organizer))
		{
			return nameof(UserRole.Organizer);
		}

		return nameof(UserRole.Customer);
	}

	/// <inheritdoc />
	public async Task<bool> EnqueueForUserAsync(Guid id, Guid userId, NotificationTemplate template, string? correlationId, CancellationToken cancellationToken = default)
	{
		if (!string.IsNullOrEmpty(correlationId))
		{
			var dup = await _db.Notifications.AnyAsync(
				q => q.TargetUserId == userId
					&& q.CorrelationId == correlationId
					&& q.AcknowledgedUtc == null,
				cancellationToken);
			if (dup)
			{
				return false;
			}
		}

		var user = await _users.GetByIdAsync(userId, cancellationToken);
		var roleLabel = user is null ? nameof(UserRole.Customer) : ResolveTargetUserRoleLabel(user.Roles);
		var templateJson = NotificationTemplateJson.Serialize(template);

		var row = new Notification
		{
			Id = id,
			CreatedUtc = DateTime.UtcNow,
			Title = string.Empty,
			Message = string.Empty,
			TemplateJson = templateJson,
			CorrelationId = correlationId,
			TargetUserId = userId,
			TargetUserRole = roleLabel,
			AcknowledgedUtc = null
		};
		await _db.Notifications.AddAsync(row, cancellationToken);
		await _db.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<IReadOnlyList<PendingNotificationItemDto>> GetUndeliveredForUserAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		return await _db.Notifications
			.AsNoTracking()
			.Where(q => q.TargetUserId == userId && q.AcknowledgedUtc == null)
			.OrderBy(q => q.CreatedUtc)
			.Select(q => new PendingNotificationItemDto(
				q.Id,
				q.Title,
				q.Message,
				q.CorrelationId,
				q.CreatedUtc,
				q.TemplateJson,
				null))
			.ToListAsync(cancellationToken);
	}

	public async Task AcknowledgeAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
	{
		var notif = await _db.Notifications.FirstOrDefaultAsync(q => q.Id == notificationId, cancellationToken);
		if (notif is null || notif.TargetUserId != userId || notif.AcknowledgedUtc != null)
		{
			return;
		}

		notif.AcknowledgedUtc = DateTime.UtcNow;
		await _db.SaveChangesAsync(cancellationToken);
	}
}
