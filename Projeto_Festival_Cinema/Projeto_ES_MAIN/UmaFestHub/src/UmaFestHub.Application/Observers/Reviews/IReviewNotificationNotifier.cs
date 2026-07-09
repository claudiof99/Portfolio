// In-app notifications: entry point invoked by ReviewService for pending moderation and author outcomes.
namespace UmaFestHub.Application.Observers.Reviews;

/// <summary>Fans out review lifecycle events to all <see cref="IReviewNotificationObserver"/> implementations.</summary>
public interface IReviewNotificationNotifier
{
	Task NotifyReviewPendingModerationAsync(ReviewPendingModerationContext context, CancellationToken cancellationToken = default);

	Task NotifyReviewAuthorOutcomeAsync(ReviewAuthorOutcomeContext context, CancellationToken cancellationToken = default);

	Task NotifyReplyPendingModerationAsync(ReplyPendingModerationContext context, CancellationToken cancellationToken = default);

	Task NotifyReplyAuthorOutcomeAsync(ReplyAuthorOutcomeContext context, CancellationToken cancellationToken = default);
}
