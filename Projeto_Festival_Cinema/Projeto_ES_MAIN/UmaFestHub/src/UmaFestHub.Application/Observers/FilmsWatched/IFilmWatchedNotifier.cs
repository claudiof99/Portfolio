namespace UmaFestHub.Application.Observers.FilmsWatched;

/// <summary>
/// Publisher (Subject) for the film-watched event. Web code calls this once after entitlement checks pass;
/// all registered <see cref="IFilmWatchedObserver"/> implementations are notified (Observer pattern).
/// </summary>
public interface IFilmWatchedNotifier
{
	/// <summary>
	/// Notifies every observer in registration order. Individual observer failures must not break the caller
	/// (the default <c>FilmWatchedNotifier</c> logs and continues per observer).
	/// </summary>
	/// <param name="context">Authenticated user and catalog <see cref="FilmWatchedContext.FilmId"/>.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	Task NotifyFilmWatchedAsync(FilmWatchedContext context, CancellationToken cancellationToken = default);
}
