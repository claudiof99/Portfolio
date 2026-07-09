namespace UmaFestHub.Application.Interfaces;

/// <summary>Scheduled pass: enqueue in-app reminders for catalog rentals whose access window ends within the configured horizon.</summary>
public interface IRentalExpiryReminderService
{
	/// <summary>Queries completed rental lines in the window and notifies through the rental-expiry notifier pipeline.</summary>
	Task EnqueueRentalExpiryRemindersAsync(CancellationToken cancellationToken = default);
}
