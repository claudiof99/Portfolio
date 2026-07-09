namespace UmaFestHub.Application.Observers.RentalExpiry;

/// <summary>Fans out rental-expiry reminders to every <see cref="IRentalExpiryObserver"/> (scheduled pass per rental line).</summary>
public interface IRentalExpiryNotifier
{
	/// <summary>Runs all observers for one snapshot row.</summary>
	Task NotifyAsync(RentalExpiryContext context, CancellationToken cancellationToken = default);
}
