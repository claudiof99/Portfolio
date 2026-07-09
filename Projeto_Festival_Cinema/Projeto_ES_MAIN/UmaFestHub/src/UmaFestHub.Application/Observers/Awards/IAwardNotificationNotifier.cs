// In-app notifications: entry point invoked by AwardService when voting is closed/deactivated.
namespace UmaFestHub.Application.Observers.Awards;

/// <summary>Fans out award lifecycle hooks to <see cref="IAwardNotificationObserver"/> implementations.</summary>
public interface IAwardNotificationNotifier
{
	Task NotifyAwardVotingClosedAsync(AwardVotingClosedContext context, CancellationToken cancellationToken = default);
}
