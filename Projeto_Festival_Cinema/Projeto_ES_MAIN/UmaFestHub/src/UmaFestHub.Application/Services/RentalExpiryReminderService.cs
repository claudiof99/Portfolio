using UmaFestHub.Application.Interfaces;
using UmaFestHub.Application.Observers.RentalExpiry;
using UmaFestHub.Domain.Interfaces;

namespace UmaFestHub.Application.Services;

/// <summary>
/// Orchestrates rental-expiry reminders: query completed rental lines in the reminder window, fan out via <see cref="IRentalExpiryNotifier"/>.
/// </summary>
public sealed class RentalExpiryReminderService : IRentalExpiryReminderService
{
	/// <summary>Same horizon as festival-ending reminders (real-time UTC window).</summary>
	internal static readonly TimeSpan ReminderWindow = TimeSpan.FromDays(3);

	private readonly IPurchaseRepository _purchases;
	private readonly IRentalExpiryNotifier _notifier;

	public RentalExpiryReminderService(IPurchaseRepository purchases, IRentalExpiryNotifier notifier)
	{
		_purchases = purchases;
		_notifier = notifier;
	}

	public async Task EnqueueRentalExpiryRemindersAsync(CancellationToken cancellationToken = default)
	{
		var now = DateTime.UtcNow;
		var rows = await _purchases.GetActiveRentalsExpiringWithinAsync(now, ReminderWindow, cancellationToken);
		if (rows.Count == 0)
		{
			return;
		}

		foreach (var row in rows)
		{
			if (row.UserId == Guid.Empty)
			{
				continue;
			}

			var remaining = row.ExpiresAtUtc - now;
			// One notifier fan-out per line; duplicate worker passes dedupe on correlation in RentalExpiryWarningObserver.
			await _notifier.NotifyAsync(
				new RentalExpiryContext
				{
					UserId = row.UserId,
					PurchaseItemId = row.PurchaseItemId,
					RentalId = row.RentalProductId,
					FilmId = row.FilmId,
					FilmTitle = row.FilmTitle,
					ExpiresAt = row.ExpiresAtUtc,
					TimeRemaining = remaining
				},
				cancellationToken);
		}
	}
}
