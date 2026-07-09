using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Observers.FestivalEnding;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

/// <summary>
/// Application orchestration for scheduled “ending soon” reminders: query festivals in window,
/// resolve purchaser user ids, notify via <see cref="IFestivalEndingNotificationNotifier"/> (Observer fan-out).
/// </summary>
public sealed class FestivalEndingReminderService : IFestivalEndingReminderService
{
	/// <summary>Real-time UTC window: end strictly after “now” and on/before now + this span (3 calendar days).</summary>
	internal static readonly TimeSpan ReminderWindow = TimeSpan.FromDays(3);

	private readonly IFestivalRepository _festivals;
	private readonly IPurchaseRepository _purchases;
	private readonly IFestivalEndingNotificationNotifier _notifier;

	public FestivalEndingReminderService(
		IFestivalRepository festivals,
		IPurchaseRepository purchases,
		IFestivalEndingNotificationNotifier notifier)
	{
		_festivals = festivals;
		_purchases = purchases;
		_notifier = notifier;
	}

	public async Task EnqueueEndingSoonNotificationsAsync(CancellationToken cancellationToken = default)
	{
		var now = DateTime.UtcNow;
		var endingSoon = await _festivals.GetFestivalsEndingWithinAsync(now, ReminderWindow, cancellationToken);
		if (endingSoon.Count == 0)
		{
			return;
		}

		// One notifier call per eligible user per festival each pass (dedupe rules live on persistence + correlation id).
		foreach (var festival in endingSoon)
		{
			var userIds = await _purchases.GetUserIdsWithCompletedPurchaseForFestivalAsync(festival.Id, cancellationToken);
			foreach (var userId in userIds)
			{
				if (userId == Guid.Empty)
				{
					continue;
				}

				await _notifier.NotifyFestivalEndingSoonAsync(
					new FestivalEndingSoonContext(festival.Id, festival.Name, festival.EndDateUtc, userId),
					cancellationToken);
			}
		}
	}
}
