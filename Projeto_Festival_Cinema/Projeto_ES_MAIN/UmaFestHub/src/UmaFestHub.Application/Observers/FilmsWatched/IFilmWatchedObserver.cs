namespace UmaFestHub.Application.Observers.FilmsWatched;

/// <summary>
/// Observer contract for the “film was watched” domain event (user opened an entitled festival watch page).
/// Part of the Observer pattern: new behaviours (loyalty, email, analytics) are added as new implementations
/// and registered in DI—no changes to <see cref="IFilmWatchedNotifier"/> or web controllers required (Open/Closed).
/// </summary>
public interface IFilmWatchedObserver
{
	/// <summary>
	/// Invoked after access has been validated; implementors perform side effects such as persisting the user’s Seen list
	/// (<c>PersonalListType.Watched</c> / PersonalLists table).
	/// </summary>
	/// <param name="context">User and catalog film identity.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	Task OnFilmWatchedAsync(FilmWatchedContext context, CancellationToken cancellationToken = default);
}
