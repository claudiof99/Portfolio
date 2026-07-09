// In-app notifications: award voting closed → per-voter messages via INotificationService.
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Observers.Awards;

/// <summary>Notifies everyone who voted when voting closes, except the user who deactivated the award.</summary>
public sealed class AwardNotificationObserver : IAwardNotificationObserver
{
	private readonly INotificationService _notifications;

	public AwardNotificationObserver(INotificationService notifications)
	{
		_notifications = notifications;
	}

	public async Task OnAwardVotingClosedAsync(AwardVotingClosedContext context, CancellationToken cancellationToken = default)
	{
		var results = context.Results
			.Select(r => new NotificationAwardResultLine(r.Label, r.Percent))
			.ToList();
		var template = NotificationTemplate.AwardResults(context.AwardName, results);
		var correlationId = $"award-results:{context.AwardId:D}";

		foreach (var userId in context.VoterUserIds)
		{
			if (userId == Guid.Empty || userId == context.DeactivatedByUserId)
			{
				continue;
			}

			await _notifications.NotifyUserAsync(userId, template, correlationId, cancellationToken);
		}
	}
}
