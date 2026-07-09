using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class FestivalRepository : IFestivalRepository
{
	private readonly AppDbContext _dbContext;

	public FestivalRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<Festival>> GetAllAsync(CancellationToken cancellationToken = default)
		=> await _dbContext.Festivals
			.Include(x => x.FestivalFilms)
			.AsNoTracking()
			.OrderBy(x => x.StartDateUtc)
			.ToListAsync(cancellationToken);

	/// <inheritdoc />
	public async Task<IReadOnlyList<Festival>> GetAllVisibleAsync(CancellationToken cancellationToken = default)
		=> await _dbContext.Festivals
			.Include(x => x.FestivalFilms)
			.AsNoTracking()
			.Where(f => !f.IsHidden)
			.OrderBy(x => x.StartDateUtc)
			.ToListAsync(cancellationToken);

	public async Task<Festival?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		   => await _dbContext.Festivals
			   .Include(x => x.FestivalFilms)
				   .ThenInclude(ff => ff.Film)
			   .AsNoTracking()
			   .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public async Task<IReadOnlyList<Festival>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
	{
		if (ids.Count == 0)
		{
			return [];
		}

		return await _dbContext.Festivals
			.AsNoTracking()
			.Where(f => ids.Contains(f.Id))
			.OrderBy(f => f.Name)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Festival>> GetFestivalsWithEndUtcInCalendarDayWindowAsync(
		DateTime utcToday,
		int maxCalendarDaysInclusive,
		CancellationToken cancellationToken = default)
	{
		var today = DateTime.SpecifyKind(utcToday.Date, DateTimeKind.Utc);
		var exclusiveUpper = today.AddDays(maxCalendarDaysInclusive + 1);

		return await _dbContext.Festivals
			.AsNoTracking()
			.Where(f => f.EndDateUtc >= today && f.EndDateUtc < exclusiveUpper)
			.OrderBy(f => f.EndDateUtc)
			.ToListAsync(cancellationToken);
	}

	/// <inheritdoc />
	/// <remarks>Used by <c>FestivalEndingReminderService</c> with a 3-day <paramref name="maxTimeUntilEndInclusive"/>.</remarks>
	public async Task<IReadOnlyList<Festival>> GetFestivalsEndingWithinAsync(
		DateTime utcNow,
		TimeSpan maxTimeUntilEndInclusive,
		CancellationToken cancellationToken = default)
	{
		if (maxTimeUntilEndInclusive < TimeSpan.Zero)
		{
			return [];
		}

		// Inclusive upper bound at instant precision (not calendar-day rounding).
		var upper = utcNow + maxTimeUntilEndInclusive;

		return await _dbContext.Festivals
			.AsNoTracking()
			.Where(f => f.EndDateUtc > utcNow && f.EndDateUtc <= upper)
			.OrderBy(f => f.EndDateUtc)
			.ToListAsync(cancellationToken);
	}

	public async Task AddAsync(Festival festival, CancellationToken cancellationToken = default)
	{
		await _dbContext.Festivals.AddAsync(festival, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Festival festival, CancellationToken cancellationToken = default)
	{
		_dbContext.Festivals.Update(festival);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var festival = await _dbContext.Festivals
			.Include(f => f.FestivalFilms)
				.ThenInclude(ff => ff.Sessions)
			.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

		if (festival is null) return;

		// ── Purchase pre-check ────────────────────────────────────────────────────
		// Check whether any PurchaseItem references a product belonging to this festival
		// (passes linked via FestivalId, tickets via Session→FestivalFilm, rentals via FestivalFilm).
		// We do this BEFORE any delete so we never produce a FK violation and can return a
		// meaningful error telling the admin to use 'Hide' instead.
		var festivalFilmIds = festival.FestivalFilms.Select(ff => ff.Id).ToList();
		var sessionIds = festival.FestivalFilms
			.SelectMany(ff => ff.Sessions)
			.Select(s => s.Id)
			.ToList();

		var hasPurchases = await _dbContext.PurchaseItems
			.AsNoTracking()
			.AnyAsync(pi =>
				// Ticket → Session → FestivalFilm → Festival
				(_dbContext.Tickets.AsNoTracking().Any(t => t.Id == pi.ProductId && sessionIds.Contains(t.SessionId))) ||
				// Rental → FestivalFilm → Festival
				(_dbContext.Rentals.AsNoTracking().Any(r => r.Id == pi.ProductId && festivalFilmIds.Contains(r.FestivalFilmId))) ||
				// Pass (DailyPass / CompletePass) → Festival
				(_dbContext.Passes.AsNoTracking().Any(p => p.Id == pi.ProductId && p.FestivalId == id)),
			cancellationToken);

		if (hasPurchases)
		{
			throw new InvalidOperationException("Festival_HasPurchases_UseHide");
		}
		// ── End purchase pre-check ────────────────────────────────────────────────

		// No purchases exist — proceed with hard delete as before.
		var tickets = await _dbContext.Tickets
			.Where(t => sessionIds.Contains(t.SessionId))
			.ToListAsync(cancellationToken);
		_dbContext.Products.RemoveRange(tickets);

		var rentals = await _dbContext.Rentals
			.Where(r => festivalFilmIds.Contains(r.FestivalFilmId))
			.ToListAsync(cancellationToken);
		_dbContext.Products.RemoveRange(rentals);

		var passes = await _dbContext.Products.OfType<Pass>()
			.Where(p => p.FestivalId == id)
			.ToListAsync(cancellationToken);
		_dbContext.Products.RemoveRange(passes);

		_dbContext.Festivals.Remove(festival);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
