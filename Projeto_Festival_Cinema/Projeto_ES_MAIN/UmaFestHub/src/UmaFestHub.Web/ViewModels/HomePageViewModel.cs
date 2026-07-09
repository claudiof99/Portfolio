namespace UmaFestHub.Web.ViewModels;

/// <summary>
/// Home page (/): featured festival, festival grids, and signed-in user carousels (watchlist, favorites, Seen).
/// The <c>WatchedFilms</c> property is populated from <c>PersonalListType.Watched</c> (same rows as <c>/watchHistory</c> and <c>/PersonalList?type=Watched</c>; rows added on entitled watch via Observer).
/// </summary>
public sealed class HomePageViewModel
{
	public IReadOnlyList<FestivalViewModel> FeaturedFestivals { get; init; } = [];
	public IReadOnlyList<FestivalViewModel> UpcomingFestivals { get; init; } = [];
	public IReadOnlyList<FestivalViewModel> NowStreamingFestivals { get; init; } = [];

	/// <summary>Films in the signed-in user’s <c>Favorites</c> list (home carousel → <c>/personal-list?type=Favorites</c>).</summary>
	public IReadOnlyList<FilmViewModel> FavoriteFilms { get; init; } = [];

	/// <summary>Films in the signed-in user’s <c>Watchlist</c> (home carousel → <c>/personal-list?type=Watchlist</c>).</summary>
	public IReadOnlyList<FilmViewModel> WatchlistFilms { get; init; } = [];

	/// <summary>Films the user opened on an entitled watch page (stored as <c>Watched</c> in PersonalLists; UI label “Seen”).</summary>
	public IReadOnlyList<FilmViewModel> WatchedFilms { get; init; } = [];

	/// <summary>Personalized recommendations for the signed-in user.</summary>
	public IReadOnlyList<RecommendationViewModel> RecommendedFilms { get; init; } = [];

	public bool IsAuthenticated { get; init; }
}
