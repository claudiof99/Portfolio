// -----------------------------------------------------------------------------
// Review replies — application port (ISP): load thread, add reply, report/approve/hide.
// Implemented by ReviewService alongside IReviewService.
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Messaging;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Interfaces;

/// <summary>Application contract for review thread replies (ISP slice separate from <see cref="IReviewService"/>).</summary>
public interface IReviewReplyService
{
	Task<IReadOnlyList<ReviewReplyDto>> GetRepliesByReviewIdsAsync(
		IReadOnlyList<Guid> reviewIds,
		Guid? viewerUserId,
		bool isModerator,
		CancellationToken cancellationToken = default);

	/// <summary>Staff moderation list: every reply for the reviews, including hidden/rejected visibility.</summary>
	Task<IReadOnlyList<ReviewReplyDto>> GetRepliesByReviewIdsForManagementAsync(
		IReadOnlyList<Guid> reviewIds,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ReviewReplyDto>> GetRepliesForReviewAsync(
		Guid festivalFilmId,
		Guid reviewId,
		Guid? viewerUserId,
		bool isModerator,
		CancellationToken cancellationToken = default);

	/// <summary>Returns (replyId, error). Error is null on success.</summary>
	Task<(Guid? ReplyId, UserMessage? Error)> AddReplyAsync(
		Guid userId,
		Guid festivalFilmId,
		Guid reviewId,
		string comment,
		ReviewStatus initialStatus,
		CancellationToken cancellationToken = default);

	Task ReportReplyAsync(Guid replyId, CancellationToken cancellationToken = default);

	Task ApproveReplyAsync(Guid replyId, CancellationToken cancellationToken = default);

	Task HideReplyAsync(Guid replyId, CancellationToken cancellationToken = default);

	Task<ReviewReplyDto?> GetReplyByIdAsync(Guid replyId, CancellationToken cancellationToken = default);
}
