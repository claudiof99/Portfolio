// -----------------------------------------------------------------------------
// Film reviews — application port (listing, create, moderation). Reply use cases:
// <see cref="IReviewReplyService"/>.
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;

namespace UmaFestHub.Application.Interfaces;

// Application-layer contract for film review use cases (listing, creation, moderation).
// Reply-specific operations live on <see cref="IReviewReplyService"/>.
public interface IReviewService
{
	Task<IReadOnlyList<ReviewDto>> GetForFestivalFilmAsync(Guid festivalFilmId, Guid? viewerUserId = null, CancellationToken cancellationToken = default);
	Task<PagedResultDto<ReviewDto>> GetForFestivalFilmPageAsync(Guid festivalFilmId, Guid? viewerUserId, int page, int pageSize, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<ManagedReviewDto>> GetAllForManagementAsync(CancellationToken cancellationToken = default);
	Task<PagedResultDto<ManagedReviewDto>> GetAllForManagementPageAsync(
		int page,
		int pageSize,
		string? movieQuery = null,
		string? authorQuery = null,
		string? status = null,
		DateTime? dayUtc = null,
		CancellationToken cancellationToken = default);
	Task<Guid> AddAsync(ReviewDto review, CancellationToken cancellationToken = default);
	Task ReportAsync(Guid reviewId, CancellationToken cancellationToken = default);
	Task HideReportedAsync(Guid reviewId, CancellationToken cancellationToken = default);
	Task ApproveAsync(Guid reviewId, CancellationToken cancellationToken = default);
}
