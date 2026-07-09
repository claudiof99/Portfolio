using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Observers.RentalExpiry;

/// <summary>
/// In-app reminder when a completed rental’s access window ends within the scheduled horizon.
/// </summary>
public sealed class RentalExpiryWarningObserver : IRentalExpiryObserver
{
	private readonly INotificationService _notifications;

	public RentalExpiryWarningObserver(INotificationService notifications)
	{
		_notifications = notifications;
	}

	public Task OnRentalExpiringAsync(RentalExpiryContext context, CancellationToken cancellationToken = default)
	{
		var template = NotificationTemplate.RentalExpiring(context.FilmTitle, context.ExpiresAt);
		var correlationId = $"rental-expiring-soon:{context.PurchaseItemId:D}";
		return _notifications.NotifyUserAsync(context.UserId, template, correlationId, cancellationToken);
	}
}
