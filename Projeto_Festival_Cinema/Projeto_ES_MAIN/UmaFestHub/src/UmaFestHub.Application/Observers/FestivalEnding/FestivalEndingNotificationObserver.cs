using UmaFestHub.Application.DTOs;
using UmaFestHub.Application.Interfaces;

namespace UmaFestHub.Application.Observers.FestivalEnding;

/// <summary>
/// Observer: turns <see cref="FestivalEndingSoonContext"/> into a pending in-app notification + live push via <see cref="INotificationService"/>.
/// </summary>
public sealed class FestivalEndingNotificationObserver : IFestivalEndingNotificationObserver
{
	private readonly INotificationService _notifications;

	public FestivalEndingNotificationObserver(INotificationService notifications)
	{
		_notifications = notifications;
	}

	public Task OnFestivalEndingSoonAsync(FestivalEndingSoonContext context, CancellationToken cancellationToken = default)
	{
		var template = NotificationTemplate.FestivalEnding(context.FestivalName, context.EndDateUtc);
		var correlationId = $"festival-ending-soon:{context.FestivalId:D}";
		return _notifications.NotifyUserAsync(context.UserId, template, correlationId, cancellationToken);
	}
}
