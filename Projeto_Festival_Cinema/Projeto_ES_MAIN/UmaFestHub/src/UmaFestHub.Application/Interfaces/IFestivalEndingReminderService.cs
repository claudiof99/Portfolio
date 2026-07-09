namespace UmaFestHub.Application.Interfaces;

/// <summary>
/// Port used by <c>FestivalEndingReminderWorker</c>: one “pass” scans DB for festivals ending within three days and
/// queues in-app notifications for users with completed purchases (pass/rental/ticket) on those festivals.
/// </summary>
public interface IFestivalEndingReminderService
{
	/// <summary>Runs a full reminder pass under the caller’s ambient cancellation (host shutdown).</summary>
	/// <remarks>Implementations resolve festivals + purchasers then call <c>IFestivalEndingNotificationNotifier</c>.</remarks>
	Task EnqueueEndingSoonNotificationsAsync(CancellationToken cancellationToken = default);
}
