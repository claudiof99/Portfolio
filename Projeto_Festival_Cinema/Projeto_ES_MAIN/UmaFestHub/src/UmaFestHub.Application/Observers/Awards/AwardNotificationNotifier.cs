// In-app notifications: fans out award hooks to all IAwardNotificationObserver implementations.
using Microsoft.Extensions.Logging;

namespace UmaFestHub.Application.Observers.Awards;

public sealed class AwardNotificationNotifier : IAwardNotificationNotifier
{
	private readonly IEnumerable<IAwardNotificationObserver> _observers;
	private readonly ILogger<AwardNotificationNotifier> _logger;

	public AwardNotificationNotifier(IEnumerable<IAwardNotificationObserver> observers, ILogger<AwardNotificationNotifier> logger)
	{
		_observers = observers;
		_logger = logger;
	}

	public async Task NotifyAwardVotingClosedAsync(AwardVotingClosedContext context, CancellationToken cancellationToken = default)
	{
		foreach (var observer in _observers)
		{
			try
			{
				await observer.OnAwardVotingClosedAsync(context, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Award notification observer {ObserverType} failed (voting closed), award {AwardId}.",
					observer.GetType().FullName, context.AwardId);
			}
		}
	}
}
