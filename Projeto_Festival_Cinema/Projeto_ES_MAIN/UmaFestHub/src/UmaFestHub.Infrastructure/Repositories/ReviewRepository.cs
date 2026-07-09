// -----------------------------------------------------------------------------
// Film reviews — EF repository. GetAllFilteredPageAsync (Manage) also OR-matches Review.Replies
// for author / status / date filters so staff can find threads via reply activity.
// -----------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
	private readonly AppDbContext _dbContext;

	public ReviewRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<Review>> GetAllAsync(CancellationToken cancellationToken = default)
		=> await _dbContext.Reviews
			.Include(x => x.Film)
			// FestivalFilm is optional on Review; EF handles null navigation safely.
			// The null-forgiving operator avoids nullable-flow warnings for ThenInclude.
			.Include(x => x.FestivalFilm!)
				.ThenInclude(x => x.Film)
			.OrderByDescending(x => x.DateUtc)
			.AsNoTracking()
			.ToListAsync(cancellationToken);

	public async Task<IReadOnlyList<Review>> GetAllPageAsync(int skip, int take, CancellationToken cancellationToken = default)
		=> await _dbContext.Reviews
			.Include(x => x.Film)
			// Same rationale as GetAllAsync: optional navigation + ThenInclude.
			.Include(x => x.FestivalFilm!)
				.ThenInclude(x => x.Film)
			.OrderByDescending(x => x.DateUtc)
			.Skip(skip)
			.Take(take)
			.AsNoTracking()
			.ToListAsync(cancellationToken);

	public async Task<IReadOnlyList<Review>> GetAllFilteredPageAsync(
		string? movieQuery,
		string? status,
		DateTime? dayUtc,
		IReadOnlyList<Guid>? userIds,
		int skip,
		int take,
		CancellationToken cancellationToken = default)
	{
		var query = _dbContext.Reviews
			.Include(x => x.Film)
			// Same rationale as above: optional navigation + ThenInclude.
			.Include(x => x.FestivalFilm!)
				.ThenInclude(x => x.Film)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(movieQuery))
		{
			// Movie filter supports either direct Film navigation or FestivalFilm->Film navigation
			// (older data may use one or the other).
			var q = movieQuery.Trim();
			query = query.Where(r =>
				(r.Film != null && r.Film.Name.Contains(q)) ||
				(r.FestivalFilm != null && r.FestivalFilm.Film != null && r.FestivalFilm.Film.Name.Contains(q)));
		}

		if (!string.IsNullOrWhiteSpace(status))
		{
			// Status filter supports special "Reported" (flag-based),
			// otherwise it maps to the stored ReviewStatus enum values.
			// Management list: match the review and/or any thread reply (same flags on ReviewReply).
			var s = status.Trim();
			if (string.Equals(s, "Reported", StringComparison.OrdinalIgnoreCase))
			{
				query = query.Where(r =>
					r.IsReported
					|| r.Replies.Any(rr => rr.IsReported));
			}
			else
			{
				if (Enum.TryParse<ReviewStatus>(s, ignoreCase: true, out var parsed))
				{
					query = query.Where(r =>
						r.Status == parsed
						|| r.Replies.Any(rr => rr.Status == parsed));
				}
			}
		}

		if (dayUtc.HasValue)
		{
			// Day filter is interpreted as a UTC day boundary: [00:00, 00:00+1day).
			var start = dayUtc.Value.Date;
			var end = start.AddDays(1);
			query = query.Where(r =>
				(r.DateUtc >= start && r.DateUtc < end)
				|| r.Replies.Any(rr => rr.DateUtc >= start && rr.DateUtc < end));
		}

		if (userIds is { Count: > 0 })
		{
			// Author name filter: review author or any reply author.
			query = query.Where(r =>
				userIds.Contains(r.UserId)
				|| r.Replies.Any(rr => userIds.Contains(rr.UserId)));
		}

		return await query
			.OrderByDescending(r => r.DateUtc)
			.Skip(skip)
			.Take(take)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Review>> GetForFestivalFilmAsync(Guid festivalFilmId, Guid? viewerUserId = null, CancellationToken cancellationToken = default)
		=> await _dbContext.Reviews
			.Where(x =>
				x.FestivalFilmId == festivalFilmId
				&& (!x.IsHiddenByAdmin || (viewerUserId.HasValue && x.UserId == viewerUserId.Value)))
			.OrderByDescending(x => x.DateUtc)
			.AsNoTracking()
			.ToListAsync(cancellationToken);

	public async Task<IReadOnlyList<Review>> GetForFestivalFilmPageAsync(Guid festivalFilmId, Guid? viewerUserId, int skip, int take, CancellationToken cancellationToken = default)
		=> await _dbContext.Reviews
			.Where(x =>
				x.FestivalFilmId == festivalFilmId
				&& (!x.IsHiddenByAdmin || (viewerUserId.HasValue && x.UserId == viewerUserId.Value)))
			.OrderByDescending(x => x.DateUtc)
			.Skip(skip)
			.Take(take)
			.AsNoTracking()
			.ToListAsync(cancellationToken);

	public async Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken = default)
		=> await _dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId, cancellationToken);

	public async Task<Review?> GetByIdWithFestivalAndFilmAsync(Guid reviewId, CancellationToken cancellationToken = default)
		=> await _dbContext.Reviews
			.AsNoTracking()
			.Include(x => x.Film)
			.Include(x => x.FestivalFilm!)
				.ThenInclude(x => x.Film)
			.Include(x => x.FestivalFilm!)
				.ThenInclude(x => x.Festival)
			.FirstOrDefaultAsync(x => x.Id == reviewId, cancellationToken);

	public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
	{
		await _dbContext.Reviews.AddAsync(review, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
	{
		_dbContext.Reviews.Update(review);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Review>> GetApprovedForFilmsAsync(
		IReadOnlyList<Guid> filmIds,
		CancellationToken cancellationToken = default)
	{
		return await _dbContext.Reviews
			.Where(r => filmIds.Contains(r.FilmId ?? Guid.Empty)
				&& r.Status == ReviewStatus.Approved
				&& !r.IsHiddenByAdmin)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}
}
