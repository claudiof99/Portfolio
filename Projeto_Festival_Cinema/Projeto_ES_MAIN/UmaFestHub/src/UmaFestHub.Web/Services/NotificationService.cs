// In-app notifications: INotificationService — persist row then SendAsync("ShowNotification", payload).
using Microsoft.AspNetCore.SignalR;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Hubs;

namespace UmaFestHub.Web.Services;

/// <summary>
/// Implements <see cref="INotificationService"/>: enqueue pending row, then <c>ShowNotification</c> to the user SignalR group.
/// Payloads carry <see cref="NotificationTemplate"/> JSON so the browser renders text in the viewer's current culture.
/// </summary>
public sealed class NotificationService : INotificationService
{
	private readonly IHubContext<NotificationHub> _hubContext;
	private readonly IPendingNotificationRepository _pending;

	public NotificationService(
		IHubContext<NotificationHub> hubContext,
		IPendingNotificationRepository pending)
	{
		_hubContext = hubContext;
		_pending = pending;
	}

	public async Task NotifyRoleAsync(string role, NotificationTemplate template, string? correlationId = null, CancellationToken cancellationToken = default)
	{
		var recipients = await _pending.EnqueueForRoleAsync(role, template, correlationId, cancellationToken);
		foreach (var (userId, notificationId) in recipients)
		{
			var payload = BuildPayload(template, correlationId, notificationId);
			await _hubContext.Clients.Group(NotificationHub.UserGroupName(userId)).SendAsync("ShowNotification", payload, cancellationToken);
		}
	}

	/// <inheritdoc />
	public async Task NotifyUserAsync(Guid userId, NotificationTemplate template, string? correlationId = null, CancellationToken cancellationToken = default)
	{
		var id = Guid.NewGuid();
		var inserted = await _pending.EnqueueForUserAsync(id, userId, template, correlationId, cancellationToken);
		if (!inserted)
		{
			return;
		}

		var payload = BuildPayload(template, correlationId, id);
		await _hubContext.Clients.Group(NotificationHub.UserGroupName(userId)).SendAsync("ShowNotification", payload, cancellationToken);
	}

	private static NotificationPayloadDto BuildPayload(NotificationTemplate template, string? correlationId, Guid id)
		=> new(
			Title: null,
			Message: null,
			CorrelationId: correlationId,
			Id: id,
			TemplateJson: NotificationTemplateJson.Serialize(template),
			CollapseGroup: template.CollapseGroup);
}
