// -----------------------------------------------------------------------------
// Awards, nominations & votes — EF implementation of IAwardRepository (paging, festival).
// -----------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class AwardRepository : IAwardRepository
{
	private readonly AppDbContext _dbContext;

	public AwardRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<(IReadOnlyList<Award> Items, bool HasNext)> GetPageWithNominationsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		if (page < 1)
		{
			page = 1;
		}

		if (pageSize < 1)
		{
			pageSize = 3;
		}

		var skip = (page - 1) * pageSize;
		var query = AwardsWithNominationsQuery()
			.Include(x => x.Festival)
			.OrderByDescending(x => x.IsActive)
			.ThenByDescending(x => x.CreatedAtUtc)
			.ThenBy(x => x.Name);

		var batch = await query.Skip(skip).Take(pageSize + 1).ToListAsync(cancellationToken);
		var hasNext = batch.Count > pageSize;
		if (hasNext)
		{
			batch = batch.Take(pageSize).ToList();
		}

		return (batch, hasNext);
	}

	public async Task<IReadOnlyList<Award>> GetAllWithNominationsAsync(CancellationToken cancellationToken = default)
		=> await AwardsWithNominationsQuery()
			.Include(x => x.Festival)
			.OrderByDescending(x => x.IsActive)
			.ThenByDescending(x => x.CreatedAtUtc)
			.ThenBy(x => x.Name)
			.ToListAsync(cancellationToken);

	public async Task<IReadOnlyList<Award>> GetByFestivalIdAsync(Guid festivalId, CancellationToken cancellationToken = default)
		=> await AwardsWithNominationsQuery()
			.Where(x => x.FestivalId == festivalId)
			.Include(x => x.Festival)
			.OrderByDescending(x => x.IsActive)
			.ThenByDescending(x => x.CreatedAtUtc)
			.ThenBy(x => x.Name)
			.ToListAsync(cancellationToken);

	private IQueryable<Award> AwardsWithNominationsQuery()
		=> _dbContext.Awards
			.Include(x => x.Nominations)
				.ThenInclude(n => n.FestivalFilm!)
				.ThenInclude(ff => ff.Film)
			.Include(x => x.Nominations)
				.ThenInclude(n => n.CreditFilm!)
				.ThenInclude(c => c.Person)
			.Include(x => x.Nominations)
				.ThenInclude(n => n.Votes)
			.AsNoTracking();

	public async Task<Award?> GetByIdAsync(Guid awardId, CancellationToken cancellationToken = default)
		=> await _dbContext.Awards.FirstOrDefaultAsync(x => x.Id == awardId, cancellationToken);

	public async Task<Award?> GetByIdWithNominationsAsync(Guid awardId, CancellationToken cancellationToken = default)
		=> await AwardsWithNominationsQuery()
			.Include(x => x.Festival)
			.FirstOrDefaultAsync(x => x.Id == awardId, cancellationToken);

	public async Task AddAsync(Award award, CancellationToken cancellationToken = default)
	{
		await _dbContext.Awards.AddAsync(award, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> TrySetIsActiveAsync(Guid awardId, bool isActive, CancellationToken cancellationToken = default)
	{
		var entity = await _dbContext.Awards.FirstOrDefaultAsync(x => x.Id == awardId, cancellationToken);
		if (entity is null)
		{
			return false;
		}

		entity.IsActive = isActive;
		await _dbContext.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<IReadOnlyList<Guid>> GetActiveAwardIdsPastEndDateAsync(DateTime utcNow, CancellationToken cancellationToken = default)
		=> await _dbContext.Awards
			.AsNoTracking()
			.Where(x => x.IsActive && x.EndDateUtc <= utcNow)
			.Select(x => x.Id)
			.ToListAsync(cancellationToken);

	public async Task<IReadOnlyDictionary<Guid, int>> GetNominationCountsByFilmIdsAsync(
		Guid festivalId,
		IReadOnlyList<Guid> filmIds,
		CancellationToken cancellationToken = default)
	{
		var counts = await _dbContext.AwardNominations
			.Where(n => n.Award != null && n.Award.FestivalId == festivalId
				&& filmIds.Contains(n.FestivalFilmId ?? Guid.Empty)
				&& n.FestivalFilmId != null)
			.GroupBy(n => n.FestivalFilmId!.Value)
			.Select(g => new { FilmId = g.Key, Count = g.Count() })
			.ToDictionaryAsync(x => x.FilmId, x => x.Count, cancellationToken);

		return counts;
	}
}
