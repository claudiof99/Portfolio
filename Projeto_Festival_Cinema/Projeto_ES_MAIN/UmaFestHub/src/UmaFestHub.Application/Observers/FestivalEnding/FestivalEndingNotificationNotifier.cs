using Microsoft.Extensions.Logging;

namespace UmaFestHub.Application.Observers.FestivalEnding;

/// <summary>
/// Subject side of the Observer pattern: invokes every registered <see cref="IFestivalEndingNotificationObserver"/>
/// for one user/festival pair; failures in one observer do not block the others.
/// </summary>
public sealed class FestivalEndingNotificationNotifier : IFestivalEndingNotificationNotifier
{
	private readonly IEnumerable<IFestivalEndingNotificationObserver> _observers;
	private readonly ILogger<FestivalEndingNotificationNotifier> _logger;

	public FestivalEndingNotificationNotifier(
		IEnumerable<IFestivalEndingNotificationObserver> observers,
		ILogger<FestivalEndingNotificationNotifier> logger)
	{
		_observers = observers;
		_logger = logger;
	}

	public async Task NotifyFestivalEndingSoonAsync(FestivalEndingSoonContext context, CancellationToken cancellationToken = default)
	{
		foreach (var observer in _observers)
		{
			try
			{
				await observer.OnFestivalEndingSoonAsync(context, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,
					"Festival ending observer {ObserverType} failed for user {UserId}, festival {FestivalId}.",
					observer.GetType().FullName, context.UserId, context.FestivalId);
			}
		}
	}
}
