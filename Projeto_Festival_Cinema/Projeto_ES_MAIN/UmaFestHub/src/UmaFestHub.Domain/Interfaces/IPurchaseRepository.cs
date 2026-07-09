using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

/// <summary>One rental access line from a completed purchase, with expiry instant for reminder scheduling.</summary>
/// <param name="UserId">Buyer who should receive the rental-expiry in-app reminder.</param>
/// <param name="PurchaseItemId">Stable id for notification correlation <c>rental-expiring-soon:{id}</c>.</param>
/// <param name="RentalProductId">Catalog rental product id (fed into rental-expiry notifier context as <c>RentalId</c>).</param>
/// <param name="FilmId">Film tied to the rental for context.</param>
/// <param name="FilmTitle">Display title in the reminder body.</param>
/// <param name="ExpiresAtUtc">Access end instant (purchase <c>DateUtc</c> + rental duration).</param>
public sealed record ActiveRentalExpiringSnapshot(
	Guid UserId,
	Guid PurchaseItemId,
	Guid RentalProductId,
	Guid FilmId,
	string FilmTitle,
	DateTime ExpiresAtUtc);

public interface IPurchaseRepository
{
	Task<IReadOnlyList<Purchase>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Purchase>> GetByUserIdExcludingExpiredAsync(Guid userId, CancellationToken cancellationToken = default);

	/// <summary>Festival ids implied by user's purchases (passes, rentals, tickets).</summary>
	Task<IReadOnlySet<Guid>> GetDistinctFestivalIdsFromUserPurchasesAsync(Guid userId, CancellationToken cancellationToken = default);

	/// <summary>Distinct users with at least one completed purchase tied to the festival (passes, rentals, tickets).</summary>
	/// <remarks>Subscriber set for festival-ending-soon notifications (scheduled pass).</remarks>
	Task<IReadOnlyList<Guid>> GetUserIdsWithCompletedPurchaseForFestivalAsync(Guid festivalId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Completed rental line items whose access window ends strictly after <paramref name="utcNow"/>
	/// and on or before <paramref name="utcNow"/> + <paramref name="maxTimeUntilExpiryInclusive"/> (purchase date + catalog rental duration).
	/// </summary>
	Task<IReadOnlyList<ActiveRentalExpiringSnapshot>> GetActiveRentalsExpiringWithinAsync(
		DateTime utcNow,
		TimeSpan maxTimeUntilExpiryInclusive,
		CancellationToken cancellationToken = default);

	Task<int> CountAsync(CancellationToken cancellationToken = default);
	Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default);
}
