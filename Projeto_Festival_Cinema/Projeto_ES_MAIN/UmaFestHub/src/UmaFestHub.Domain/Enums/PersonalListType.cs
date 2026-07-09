namespace UmaFestHub.Domain.Entities;

/// <summary>
/// Discriminator for per-user film lists. Persisted as string via EF Core value conversion on PersonalList.Type.
/// Bind from query string as <c>?type=Watchlist|Favorites|Watched</c> on GET <c>/personal-list</c>; <c>Watched</c> is also the list type for <c>/watchHistory</c> (Seen).
/// </summary>
public enum PersonalListType
{
	Watchlist = 0,
	Favorites = 1,
	/// <summary>
	/// “Seen” in the UI: films the user opened on an entitled festival watch page (appended by the film-watched Observer in the Application layer)
	/// or removed via <c>/PersonalList/Remove</c>. Stored in <c>PersonalLists</c>.
	/// </summary>
	Watched = 2
}
