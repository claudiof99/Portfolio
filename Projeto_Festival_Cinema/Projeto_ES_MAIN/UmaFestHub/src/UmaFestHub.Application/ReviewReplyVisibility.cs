// -----------------------------------------------------------------------------
// Review replies — public visibility rules for reply DTOs (used by ReviewService filters).
// -----------------------------------------------------------------------------
using UmaFestHub.Application.DTOs;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application;

/// <summary>Same visibility rules as public review cards (approved / pending for author+mods / rejected for author).</summary>
public static class ReviewReplyVisibility
{
	public static bool CanSee(ReviewReplyDto reply, Guid? viewerUserId, bool isModerator)
	{
		var isApproved = string.Equals(reply.Status, ReviewStatus.Approved.ToString(), StringComparison.OrdinalIgnoreCase);
		var isPending = string.Equals(reply.Status, ReviewStatus.Pending.ToString(), StringComparison.OrdinalIgnoreCase);
		var isRejected = string.Equals(reply.Status, ReviewStatus.Rejected.ToString(), StringComparison.OrdinalIgnoreCase);
		var isAuthor = viewerUserId.HasValue && viewerUserId.Value == reply.UserId;

		return isApproved
			|| (isPending && (isAuthor || isModerator))
			|| (isRejected && isAuthor);
	}
}
