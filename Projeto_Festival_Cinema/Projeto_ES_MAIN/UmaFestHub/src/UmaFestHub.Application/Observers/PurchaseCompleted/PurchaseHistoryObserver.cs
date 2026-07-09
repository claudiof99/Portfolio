using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Observers.PurchaseCompleted;

/// <summary>
/// Observer: after a successful checkout, enqueues an in-app row and pushes <c>ShowNotification</c> via <see cref="INotificationService"/>.
/// </summary>
public sealed class PurchaseHistoryObserver : IPurchaseCompletedObserver
{
	private readonly INotificationService _notifications;

	public PurchaseHistoryObserver(INotificationService notifications)
	{
		_notifications = notifications;
	}

	public Task OnPurchaseCompletedAsync(PurchaseCompletedContext context, CancellationToken cancellationToken = default)
	{
		var template = NotificationTemplate.PurchaseCompleted(context.TotalAmount);
		var correlationId = $"purchase-completed:{context.PurchaseId:D}";
		return _notifications.NotifyUserAsync(context.UserId, template, correlationId, cancellationToken);
	}
}
