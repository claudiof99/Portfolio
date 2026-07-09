// In-app notifications: HTTP pull for pending rows + POST ack (used by signalr-notifications.js).
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Web.Extensions;

namespace UmaFestHub.Web.Controllers;

/// <summary>Pull-model notification replay after login (queued rows + ack).</summary>
[Authorize]
public sealed class NotificationsController : Controller
{
	private readonly IPendingNotificationRepository _pending;

	public NotificationsController(IPendingNotificationRepository pending)
	{
		_pending = pending;
	}

	[HttpGet("/notifications/pending")]
	public async Task<IActionResult> Pending(CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
		{
			return Unauthorized();
		}

		var items = await _pending.GetUndeliveredForUserAsync(userId, cancellationToken);
		var rendered = items.Select(ToClientDto).ToList();
		return Json(rendered);
	}

	[HttpPost("/notifications/ack")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Ack([FromForm] Guid id, CancellationToken cancellationToken)
	{
		if (!User.TryGetCurrentUserId(out var userId))
		{
			return Unauthorized();
		}

		await _pending.AcknowledgeAsync(userId, id, cancellationToken);
		return Ok();
	}

	private static ClientNotificationDto ToClientDto(PendingNotificationItemDto item)
	{
		var template = NotificationTemplateJson.Deserialize(item.TemplateJson);
		if (template is not null)
		{
			return new ClientNotificationDto(
				item.Id,
				string.Empty,
				string.Empty,
				item.CorrelationId,
				template.CollapseGroup,
				item.TemplateJson);
		}

		return new ClientNotificationDto(item.Id, item.Title, item.Message, item.CorrelationId, item.CollapseGroup);
	}
}
