// In-app notifications: fans out review hooks to all IReviewNotificationObserver implementations.
using Microsoft.Extensions.Logging;

namespace UmaFestHub.Application.Observers.Reviews;

public sealed class ReviewNotificationNotifier : IReviewNotificationNotifier
{
	private readonly IEnumerable<IReviewNotificationObserver> _observers;
	private readonly ILogger<ReviewNotificationNotifier> _logger;

	public ReviewNotificationNotifier(IEnumerable<IReviewNotificationObserver> observers, ILogger<ReviewNotificationNotifier> logger)
	{
		_observers = observers;
		_logger = logger;
	}

	public async Task NotifyReviewPendingModerationAsync(ReviewPendingModerationContext context, CancellationToken cancellationToken = default)
	{
		foreach (var observer in _observers)
		{
			try
			{
				await observer.OnReviewPendingModerationAsync(context, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Review notification observer {ObserverType} failed (pending moderation), review {ReviewId}.",
					observer.GetType().FullName, context.ReviewId);
			}
		}
	}

	public async Task NotifyReviewAuthorOutcomeAsync(ReviewAuthorOutcomeContext context, CancellationToken cancellationToken = default)
	{
		foreach (var observer in _observers)
		{
			try
			{
				await observer.OnReviewAuthorOutcomeAsync(context, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Review notification observer {ObserverType} failed (author outcome), review {ReviewId}.",
					observer.GetType().FullName, context.ReviewId);
			}
		}
	}

	public async Task NotifyReplyPendingModerationAsync(ReplyPendingModerationContext context, CancellationToken cancellationToken = default)
	{
		foreach (var observer in _observers)
		{
			try
			{
				await observer.OnReplyPendingModerationAsync(context, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Review notification observer {ObserverType} failed (reply pending moderation), reply {ReplyId}.",
					observer.GetType().FullName, context.ReplyId);
			}
		}
	}

	public async Task NotifyReplyAuthorOutcomeAsync(ReplyAuthorOutcomeContext context, CancellationToken cancellationToken = default)
	{
		foreach (var observer in _observers)
		{
			try
			{
				await observer.OnReplyAuthorOutcomeAsync(context, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Review notification observer {ObserverType} failed (reply author outcome), reply {ReplyId}.",
					observer.GetType().FullName, context.ReplyId);
			}
		}
	}
}
