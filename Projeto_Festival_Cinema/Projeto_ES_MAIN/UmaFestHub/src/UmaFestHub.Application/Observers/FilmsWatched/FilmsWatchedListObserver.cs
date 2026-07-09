using UmaFestHub.Application.Interfaces;
using UmaFestHub.Domain.Entities;

namespace UmaFestHub.Application.Observers.FilmsWatched;

/// <summary>
/// Concrete <see cref="IFilmWatchedObserver"/> for the Seen list: adds the catalog film to the user’s
/// <c>PersonalLists</c> row with <see cref="PersonalListType.Watched"/> (UI label “Seen”). Duplicate rows are
/// prevented by <c>IPersonalListRepository.AddAsync</c>.
/// </summary>
public sealed class FilmsWatchedListObserver : IFilmWatchedObserver
{
	private readonly IPersonalListService _personalListService;

	/// <param name="personalListService">Application service used to append list membership.</param>
	public FilmsWatchedListObserver(IPersonalListService personalListService)
	{
		_personalListService = personalListService;
	}

	/// <inheritdoc />
	public Task OnFilmWatchedAsync(FilmWatchedContext context, CancellationToken cancellationToken = default)
		=> _personalListService.AddFilmAsync(context.UserId, PersonalListType.Watched, context.FilmId, cancellationToken);
}
