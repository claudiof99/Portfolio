using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Application.Helpers;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Domain.ValueObjects;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
	private readonly AppDbContext _dbContext;

	public PurchaseRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<Purchase>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
		=> await _dbContext.Purchases
			.Include(x => x.PurchaseItems)
				.ThenInclude(x => x.Product)
			.AsNoTracking()
			.Where(x => x.UserId == userId)
			.OrderByDescending(x => x.DateUtc)
			.ToListAsync(cancellationToken);

	public async Task<IReadOnlyList<Purchase>> GetByUserIdExcludingExpiredAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var purchases = await _dbContext.Purchases
			.Include(x => x.PurchaseItems)
				.ThenInclude(pi => pi.Product)
					.ThenInclude(p => (p as Ticket)!.Session)
			.AsNoTracking()
			.Where(x => x.UserId == userId)
			.OrderByDescending(x => x.DateUtc)
			.ToListAsync(cancellationToken);

		var now = DateTime.UtcNow;
		foreach (var purchase in purchases)
		{
			// Since the entities are loaded AsNoTracking, we can't use RemoveAll.
			// Instead, we replace the collection with a new filtered list.
			var nonExpiredItems = new List<PurchaseItem>();
			foreach (var item in purchase.PurchaseItems)
			{
				if (item.Product is Rental rental)
				{
					if (!ExpirationCalculator.IsRentalExpired(purchase.DateUtc, rental.Duration, now))
					{
						nonExpiredItems.Add(item);
					}
				}
				else if (item.Product is Ticket ticket)
				{
					// A ticket is for a session; check if the session has ended.
					if (ticket.Session != null && !ExpirationCalculator.IsSessionExpired(ticket.Session.EndTimeUtc, now))
					{
						nonExpiredItems.Add(item);
					}
				}
				else
				{
					nonExpiredItems.Add(item);
				}
			}
			purchase.PurchaseItems = nonExpiredItems;
		}

		return purchases;
	}

	public async Task<IReadOnlySet<Guid>> GetDistinctFestivalIdsFromUserPurchasesAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var purchases = _dbContext.Purchases.Where(p => p.UserId == userId);

		var dailyPassIds = from pu in purchases
			from pi in pu.PurchaseItems
			join d in _dbContext.DailyPasses on pi.ProductId equals d.Id
			select d.FestivalId;

		var completePassIds = from pu in purchases
			from pi in pu.PurchaseItems
			join c in _dbContext.CompletePasses on pi.ProductId equals c.Id
			select c.FestivalId;

		var rentalIds = from pu in purchases
			from pi in pu.PurchaseItems
			join r in _dbContext.Rentals on pi.ProductId equals r.Id
			join ff in _dbContext.FestivalFilms on r.FestivalFilmId equals ff.Id
			select ff.FestivalId;

		var ticketIds = from pu in purchases
			from pi in pu.PurchaseItems
			join t in _dbContext.Tickets on pi.ProductId equals t.Id
			join s in _dbContext.Sessions on t.SessionId equals s.Id
			join ff in _dbContext.FestivalFilms on s.FestivalFilmId equals ff.Id
			select ff.FestivalId;

		var allIds = await dailyPassIds
			.Union(completePassIds)
			.Union(rentalIds)
			.Union(ticketIds)
			.Distinct()
			.ToListAsync(cancellationToken);

		return new HashSet<Guid>(allIds);
	}

	/// <inheritdoc />
	/// <remarks>Used by festival-ending reminder: only <see cref="PurchaseStatus.Completed"/> purchases qualify.</remarks>
	public async Task<IReadOnlyList<Guid>> GetUserIdsWithCompletedPurchaseForFestivalAsync(Guid festivalId, CancellationToken cancellationToken = default)
	{
		var purchases = _dbContext.Purchases.Where(p => p.Status == PurchaseStatus.Completed);

		var fromDaily = from pu in purchases
			from pi in pu.PurchaseItems
			join d in _dbContext.DailyPasses on pi.ProductId equals d.Id
			where d.FestivalId == festivalId
			select pu.UserId;

		var fromComplete = from pu in purchases
			from pi in pu.PurchaseItems
			join c in _dbContext.CompletePasses on pi.ProductId equals c.Id
			where c.FestivalId == festivalId
			select pu.UserId;

		var fromRental = from pu in purchases
			from pi in pu.PurchaseItems
			join r in _dbContext.Rentals on pi.ProductId equals r.Id
			join ff in _dbContext.FestivalFilms on r.FestivalFilmId equals ff.Id
			where ff.FestivalId == festivalId
			select pu.UserId;

		var fromTicket = from pu in purchases
			from pi in pu.PurchaseItems
			join t in _dbContext.Tickets on pi.ProductId equals t.Id
			join s in _dbContext.Sessions on t.SessionId equals s.Id
			join ff in _dbContext.FestivalFilms on s.FestivalFilmId equals ff.Id
			where ff.FestivalId == festivalId
			select pu.UserId;

		return await fromDaily
			.Union(fromComplete)
			.Union(fromRental)
			.Union(fromTicket)
			.Distinct()
			.ToListAsync(cancellationToken);
	}

	/// <inheritdoc />
	public async Task<IReadOnlyList<ActiveRentalExpiringSnapshot>> GetActiveRentalsExpiringWithinAsync(
		DateTime utcNow,
		TimeSpan maxTimeUntilExpiryInclusive,
		CancellationToken cancellationToken = default)
	{
		if (maxTimeUntilExpiryInclusive < TimeSpan.Zero)
		{
			return [];
		}

		var upper = utcNow + maxTimeUntilExpiryInclusive;

		var rows = await (
			from pu in _dbContext.Purchases.AsNoTracking()
			where pu.Status == PurchaseStatus.Completed
			from pi in _dbContext.PurchaseItems.AsNoTracking()
			where pi.PurchaseId == pu.Id
			join r in _dbContext.Rentals.AsNoTracking() on pi.ProductId equals r.Id
			join ff in _dbContext.FestivalFilms.AsNoTracking() on r.FestivalFilmId equals ff.Id
			join f in _dbContext.Films.AsNoTracking() on ff.FilmId equals f.Id
			select new
			{
				pu.UserId,
				PurchaseItemId = pi.Id,
				r.Id,
				FilmId = f.Id,
				FilmTitle = f.Name,
				pu.DateUtc,
				r.Duration
			}).ToListAsync(cancellationToken);

		var results = new List<ActiveRentalExpiringSnapshot>();
		foreach (var row in rows)
		{
			// Expiry = purchase instant + catalog duration; filter to (utcNow, upper] in memory (EF shape loads candidates).
			var expires = ComputeRentalExpiryUtc(row.DateUtc, row.Duration);
			if (expires > utcNow && expires <= upper)
			{
				var title = string.IsNullOrWhiteSpace(row.FilmTitle) ? "this film" : row.FilmTitle.Trim();
				results.Add(new ActiveRentalExpiringSnapshot(
					row.UserId,
					row.PurchaseItemId,
					row.Id,
					row.FilmId,
					title,
					expires));
			}
		}

		return results;
	}

	private static DateTime ComputeRentalExpiryUtc(DateTime purchaseDateUtc, Duration duration)
	{
		// Mirrors rental access end used by entitlements and by RentalExpiryReminderService.
		return duration.Unit switch
		{
			DurationUnit.Hours => purchaseDateUtc.AddHours(duration.Value),
			DurationUnit.Days => purchaseDateUtc.AddDays(duration.Value),
			DurationUnit.Minutes => purchaseDateUtc.AddMinutes(duration.Value),
			_ => purchaseDateUtc
		};
	}

	public async Task<int> CountAsync(CancellationToken cancellationToken = default)
		=> await _dbContext.Purchases.CountAsync(cancellationToken);

	public async Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default)
	{
		await _dbContext.Purchases.AddAsync(purchase, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}