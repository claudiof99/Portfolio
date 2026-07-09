namespace UmaFestHub.Application.Observers.RentalExpiry;

/// <summary>One scheduled reminder for a rental line item (catalog product + purchase line + expiry instant).</summary>
public sealed class RentalExpiryContext
{
	/// <summary>Buyer receiving the reminder.</summary>
	public required Guid UserId { get; init; }
	/// <summary>Purchase line id — stable key for correlation / dedupe.</summary>
	public required Guid PurchaseItemId { get; init; }
	/// <summary>Catalog rental product id (from snapshot <c>RentalProductId</c>).</summary>
	public required Guid RentalId { get; init; }
	/// <summary>Film id for deep links or future use.</summary>
	public required Guid FilmId { get; init; }
	/// <summary>Human-readable film title in the notification body.</summary>
	public required string FilmTitle { get; init; }
	/// <summary>Access end instant in UTC (shown in the message).</summary>
	public required DateTime ExpiresAt { get; init; }
	/// <summary>Time from “now” at enqueue to <see cref="ExpiresAt"/>; used for phrasing “less than N days”.</summary>
	public required TimeSpan TimeRemaining { get; init; }
}
