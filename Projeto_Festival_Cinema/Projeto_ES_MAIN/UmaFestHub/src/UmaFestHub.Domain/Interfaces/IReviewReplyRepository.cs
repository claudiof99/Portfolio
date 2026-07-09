// -----------------------------------------------------------------------------
// Review replies — persistence port for ReviewReply rows (thread + management queries).
// -----------------------------------------------------------------------------
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Domain.Interfaces;

/// <summary>
/// Persistence for <see cref="ReviewReply"/> rows (thread under a film review).
/// </summary>
public interface IReviewReplyRepository
{
	Task<IReadOnlyList<ReviewReply>> GetForReviewIdsAsync(IReadOnlyList<Guid> reviewIds, Guid? viewerUserId, CancellationToken cancellationToken = default);

	/// <summary>All replies for the given reviews (staff moderation; includes admin-hidden rows).</summary>
	Task<IReadOnlyList<ReviewReply>> GetForReviewIdsForManagementAsync(IReadOnlyList<Guid> reviewIds, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ReviewReply>> GetForReviewIdAsync(Guid reviewId, Guid? viewerUserId, CancellationToken cancellationToken = default);

	Task<ReviewReply?> GetByIdAsync(Guid replyId, CancellationToken cancellationToken = default);

	Task<ReviewReply?> GetByIdWithReviewFestivalAndFilmAsync(Guid replyId, CancellationToken cancellationToken = default);

	Task AddAsync(ReviewReply reply, CancellationToken cancellationToken = default);

	Task UpdateAsync(ReviewReply reply, CancellationToken cancellationToken = default);
}
