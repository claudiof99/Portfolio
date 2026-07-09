namespace UmaFestHub.Application.Observers.Awards;

/// <summary>In-app notification reactions to award lifecycle (extend with extra observers via DI).</summary>
public interface IAwardNotificationObserver
{
	Task OnAwardVotingClosedAsync(AwardVotingClosedContext context, CancellationToken cancellationToken = default);
}
