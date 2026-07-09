// In-app notifications: review lifecycle → INotificationService (staff pending digest + author outcomes).
using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Enums;

namespace UmaFestHub.Application.Observers.Reviews;

/// <summary>Maps review domain events to <see cref="INotificationService"/> (Admin/Organizer for pending; author for moderation outcomes).</summary>
public sealed class ReviewNotificationObserver : IReviewNotificationObserver
{
	private readonly INotificationService _notifications;

	public ReviewNotificationObserver(INotificationService notifications)
	{
		_notifications = notifications;
	}

	public async Task OnReviewPendingModerationAsync(ReviewPendingModerationContext context, CancellationToken cancellationToken = default)
	{
		var template = NotificationTemplate.ReviewPending();
		var correlationId = context.ReviewId.ToString("D");

		await _notifications.NotifyRoleAsync(UserRole.Admin.ToString(), template, correlationId, cancellationToken);
		await _notifications.NotifyRoleAsync(UserRole.Organizer.ToString(), template, correlationId, cancellationToken);
	}

	public Task OnReviewAuthorOutcomeAsync(ReviewAuthorOutcomeContext context, CancellationToken cancellationToken = default)
	{
		var template = NotificationTemplate.ReviewOutcome(
			context.IsApproved,
			context.FestivalName,
			context.FilmTitle,
			context.Rating,
			context.ReviewComment);
		var correlationId = $"review-author-outcome:{context.ReviewId:D}:{(context.IsApproved ? "a" : "r")}";
		return _notifications.NotifyUserAsync(context.AuthorUserId, template, correlationId, cancellationToken);
	}

	public async Task OnReplyPendingModerationAsync(ReplyPendingModerationContext context, CancellationToken cancellationToken = default)
	{
		var template = NotificationTemplate.ReplyPending();
		var correlationId = context.ReplyId.ToString("D");

		await _notifications.NotifyRoleAsync(UserRole.Admin.ToString(), template, correlationId, cancellationToken);
		await _notifications.NotifyRoleAsync(UserRole.Organizer.ToString(), template, correlationId, cancellationToken);
	}

	public Task OnReplyAuthorOutcomeAsync(ReplyAuthorOutcomeContext context, CancellationToken cancellationToken = default)
	{
		var template = NotificationTemplate.ReplyOutcome(
			context.IsApproved,
			context.FestivalName,
			context.FilmTitle,
			context.ReplyComment);
		var correlationId = $"reply-author-outcome:{context.ReplyId:D}:{(context.IsApproved ? "a" : "r")}";
		return _notifications.NotifyUserAsync(context.AuthorUserId, template, correlationId, cancellationToken);
	}
}
