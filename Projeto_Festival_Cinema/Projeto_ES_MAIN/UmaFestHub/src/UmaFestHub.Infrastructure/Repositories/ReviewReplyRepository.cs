// -----------------------------------------------------------------------------
// Review replies — EF implementation of IReviewReplyRepository (public vs staff queries).
// -----------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using UmaFestHub.Domain.Entities;
using UmaFestHub.Domain.Interfaces;
using UmaFestHub.Infrastructure.Data;

namespace UmaFestHub.Infrastructure.Repositories;

public class ReviewReplyRepository : IReviewReplyRepository
{
	private readonly AppDbContext _dbContext;

	public ReviewReplyRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<ReviewReply>> GetForReviewIdsAsync(IReadOnlyList<Guid> reviewIds, Guid? viewerUserId, CancellationToken cancellationToken = default)
	{
		if (reviewIds.Count == 0)
		{
			return Array.Empty<ReviewReply>();
		}

		var query = _dbContext.ReviewReplies
			.Where(x => reviewIds.Contains(x.ReviewId))
			.Where(x => !x.IsHiddenByAdmin || (viewerUserId.HasValue && x.UserId == viewerUserId.Value));

		return await query
			.OrderBy(x => x.DateUtc)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<ReviewReply>> GetForReviewIdsForManagementAsync(IReadOnlyList<Guid> reviewIds, CancellationToken cancellationToken = default)
	{
		if (reviewIds.Count == 0)
		{
			return Array.Empty<ReviewReply>();
		}

		return await _dbContext.ReviewReplies
			.Where(x => reviewIds.Contains(x.ReviewId))
			.OrderBy(x => x.DateUtc)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<ReviewReply>> GetForReviewIdAsync(Guid reviewId, Guid? viewerUserId, CancellationToken cancellationToken = default)
	{
		var query = _dbContext.ReviewReplies
			.Where(x => x.ReviewId == reviewId)
			.Where(x => !x.IsHiddenByAdmin || (viewerUserId.HasValue && x.UserId == viewerUserId.Value));

		return await query
			.OrderBy(x => x.DateUtc)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task<ReviewReply?> GetByIdAsync(Guid replyId, CancellationToken cancellationToken = default)
		=> await _dbContext.ReviewReplies.FirstOrDefaultAsync(x => x.Id == replyId, cancellationToken);

	public async Task<ReviewReply?> GetByIdWithReviewFestivalAndFilmAsync(Guid replyId, CancellationToken cancellationToken = default)
		=> await _dbContext.ReviewReplies
			.AsNoTracking()
			.Include(x => x.Review!)
				.ThenInclude(r => r.Film)
			.Include(x => x.Review!)
				.ThenInclude(r => r.FestivalFilm!)
					.ThenInclude(ff => ff.Film)
			.Include(x => x.Review!)
				.ThenInclude(r => r.FestivalFilm!)
					.ThenInclude(ff => ff.Festival)
			.FirstOrDefaultAsync(x => x.Id == replyId, cancellationToken);

	public async Task AddAsync(ReviewReply reply, CancellationToken cancellationToken = default)
	{
		await _dbContext.ReviewReplies.AddAsync(reply, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(ReviewReply reply, CancellationToken cancellationToken = default)
	{
		_dbContext.ReviewReplies.Update(reply);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
