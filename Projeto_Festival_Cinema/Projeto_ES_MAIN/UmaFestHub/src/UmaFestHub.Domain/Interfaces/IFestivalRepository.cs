using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

public interface IFestivalRepository
{
	Task<IReadOnlyList<Festival>> GetAllAsync(CancellationToken cancellationToken = default);

	/// <summary>Returns only festivals where <see cref="Festival.IsHidden"/> is false. Used exclusively by the public Browse page.</summary>
	Task<IReadOnlyList<Festival>> GetAllVisibleAsync(CancellationToken cancellationToken = default);

	Task<Festival?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Festival>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

	/// <summary>
	/// Festivals whose end instant is still on or after <paramref name="utcToday"/> (UTC date start)
	/// and strictly before <paramref name="utcToday"/> + (<paramref name="maxCalendarDaysInclusive"/> + 1) calendar days
	/// (i.e. end falls within the next <paramref name="maxCalendarDaysInclusive"/> end-dates inclusive, by UTC calendar day).
	/// </summary>
	Task<IReadOnlyList<Festival>> GetFestivalsWithEndUtcInCalendarDayWindowAsync(
		DateTime utcToday,
		int maxCalendarDaysInclusive,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Festivals whose <see cref="Festival.EndDateUtc"/> is strictly after <paramref name="utcNow"/>
	/// and on or before <paramref name="utcNow"/> + <paramref name="maxTimeUntilEndInclusive"/> (real-time UTC comparison).
	/// </summary>
	/// <remarks>Supports the hosted “festival ending within 3 days” reminder workflow.</remarks>
	Task<IReadOnlyList<Festival>> GetFestivalsEndingWithinAsync(
		DateTime utcNow,
		TimeSpan maxTimeUntilEndInclusive,
		CancellationToken cancellationToken = default);

	Task AddAsync(Festival festival, CancellationToken cancellationToken = default);

	Task UpdateAsync(Festival festival, CancellationToken cancellationToken = default);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
