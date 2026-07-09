namespace UmaFestHub.Application.Observers.FestivalEnding;

/// <summary>
/// Fans out festival-ending-soon reminders to every <see cref="IFestivalEndingNotificationObserver"/>.
/// Register additional observers in DI without changing callers.
/// </summary>
public interface IFestivalEndingNotificationNotifier
{
	/// <param name="context">Single (user × festival) notification opportunity from the scheduler pass.</param>
	Task NotifyFestivalEndingSoonAsync(FestivalEndingSoonContext context, CancellationToken cancellationToken = default);
}
