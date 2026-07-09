namespace UmaFestHub.Application.Observers.Reviews;

/// <summary>In-app notification reactions to review lifecycle (extend with extra observers via DI).</summary>
public interface IReviewNotificationObserver
{
	Task OnReviewPendingModerationAsync(ReviewPendingModerationContext context, CancellationToken cancellationToken = default);

	Task OnReviewAuthorOutcomeAsync(ReviewAuthorOutcomeContext context, CancellationToken cancellationToken = default);

	Task OnReplyPendingModerationAsync(ReplyPendingModerationContext context, CancellationToken cancellationToken = default);

	Task OnReplyAuthorOutcomeAsync(ReplyAuthorOutcomeContext context, CancellationToken cancellationToken = default);
}
