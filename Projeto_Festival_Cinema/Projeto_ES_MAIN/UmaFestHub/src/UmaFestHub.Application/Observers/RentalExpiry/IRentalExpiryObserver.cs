namespace UmaFestHub.Application.Observers.RentalExpiry;

/// <summary>Reacts when a completed rental’s access end is within the reminder window (e.g. enqueue in-app notification).</summary>
public interface IRentalExpiryObserver
{
	/// <summary>Called once per qualifying rental line per worker pass (dedupe is correlation-based upstream).</summary>
	Task OnRentalExpiringAsync(RentalExpiryContext context, CancellationToken cancellationToken = default);
}
