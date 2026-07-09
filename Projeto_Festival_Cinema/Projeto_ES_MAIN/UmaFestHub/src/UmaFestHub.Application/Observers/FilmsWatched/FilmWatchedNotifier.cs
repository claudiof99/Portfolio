using Microsoft.Extensions.Logging;

namespace UmaFestHub.Application.Observers.FilmsWatched;

/// <summary>
/// Default <see cref="IFilmWatchedNotifier"/> implementation: dispatches <see cref="FilmWatchedContext"/> to every
/// scoped <see cref="IFilmWatchedObserver"/> (Observer / multicast). Keeps playback working if one side effect fails.
/// </summary>
public sealed class FilmWatchedNotifier : IFilmWatchedNotifier
{
	private readonly IEnumerable<IFilmWatchedObserver> _observers;
	private readonly ILogger<FilmWatchedNotifier> _logger;

	/// <summary>
	/// <paramref name="observers"/> is injected as all <see cref="IFilmWatchedObserver"/> registrations (e.g. Seen list, future loyalty).
	/// </summary>
	public FilmWatchedNotifier(IEnumerable<IFilmWatchedObserver> observers, ILogger<FilmWatchedNotifier> logger)
	{
		_observers = observers;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task NotifyFilmWatchedAsync(FilmWatchedContext context, CancellationToken cancellationToken = default)
	{
		foreach (var observer in _observers)
		{
			try
			{
				await observer.OnFilmWatchedAsync(context, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Film watched observer {ObserverType} failed for user {UserId}, film {FilmId}.",
					observer.GetType().FullName, context.UserId, context.FilmId);
			}
		}
	}
}
