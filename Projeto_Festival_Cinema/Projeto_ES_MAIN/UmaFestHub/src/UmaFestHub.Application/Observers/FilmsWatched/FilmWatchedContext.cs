namespace UmaFestHub.Application.Observers.FilmsWatched;

/// <summary>
/// Immutable payload for the film-watched notification: raised when an authenticated user successfully opens
/// a festival watch URL (session or pass/rental film view) after access validation.
/// </summary>
/// <param name="UserId">Internal user identifier.</param>
/// <param name="FilmId">Catalog film id (same as <c>Film.Id</c> used by <c>PersonalLists.FilmId</c> for Seen).</param>
public sealed record FilmWatchedContext(Guid UserId, Guid FilmId);
